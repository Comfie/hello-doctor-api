using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByStatus;

public class GetPrescriptionsByStatusHandler : IRequestHandler<GetPrescriptionsByStatusQuery, Result<PaginatedPrescriptionsResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _dateTime;
    private readonly IUser _user;

    public GetPrescriptionsByStatusHandler(IApplicationDbContext db, IDateTimeService dateTime, IUser user)
    {
        _db = db;
        _dateTime = dateTime;
        _user = user;
    }

    public async Task<Result<PaginatedPrescriptionsResponse>> Handle(GetPrescriptionsByStatusQuery request, CancellationToken ct)
    {
        // Validate pharmacy access
        var userPharmacyId = _user.GetPharmacyId();
        if (userPharmacyId.HasValue && userPharmacyId.Value != request.PharmacyId)
        {
            return Result<PaginatedPrescriptionsResponse>.Forbidden();
        }

        var now = _dateTime.OffsetNow;
        var query = _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId && p.Status == request.Status);

        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var prescriptions = await query
            .OrderByDescending(p => p.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PrescriptionListItem(
                p.Id,
                "RX-" + p.Id.ToString(),
                p.MainMember.FirstName + " " + p.MainMember.LastName,
                p.Beneficiary.FirstName + " " + p.Beneficiary.LastName,
                p.IssuedDate,
                p.ExpiryDate,
                p.Status.ToString(),
                p.Notes,
                p.ExpiryDate < now,
                p.PrescriptionFiles != null ? p.PrescriptionFiles.Count : 0
            ))
            .ToListAsync(ct);

        var response = new PaginatedPrescriptionsResponse(
            prescriptions,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );

        return Result.Success(response);
    }
}
