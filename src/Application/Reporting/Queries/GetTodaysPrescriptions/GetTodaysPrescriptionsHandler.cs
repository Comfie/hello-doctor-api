using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetTodaysPrescriptions;

public class GetTodaysPrescriptionsHandler : IRequestHandler<GetTodaysPrescriptionsQuery, Result<List<PrescriptionListItem>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _dateTime;
    private readonly IUser _user;

    public GetTodaysPrescriptionsHandler(IApplicationDbContext db, IDateTimeService dateTime, IUser user)
    {
        _db = db;
        _dateTime = dateTime;
        _user = user;
    }

    public async Task<Result<List<PrescriptionListItem>>> Handle(GetTodaysPrescriptionsQuery request, CancellationToken ct)
    {
        // Validate pharmacy access
        var userPharmacyId = _user.GetPharmacyId();
        if (userPharmacyId.HasValue && userPharmacyId.Value != request.PharmacyId)
        {
            return Result<List<PrescriptionListItem>>.Forbidden();
        }

        var todayStart = _dateTime.OffsetNow.UtcDateTime.Date;
        var now = _dateTime.OffsetNow.UtcDateTime;

        var prescriptions = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId && p.Created >= todayStart)
            .OrderByDescending(p => p.Created)
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

        return Result.Success(prescriptions);
    }
}
