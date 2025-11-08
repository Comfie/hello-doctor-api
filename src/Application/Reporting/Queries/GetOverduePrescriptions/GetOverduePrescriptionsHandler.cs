using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetOverduePrescriptions;

public class GetOverduePrescriptionsHandler : IRequestHandler<GetOverduePrescriptionsQuery, Result<List<PrescriptionListItem>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _dateTime;
    private readonly IUser _user;

    public GetOverduePrescriptionsHandler(IApplicationDbContext db, IDateTimeService dateTime, IUser user)
    {
        _db = db;
        _dateTime = dateTime;
        _user = user;
    }

    public async Task<Result<List<PrescriptionListItem>>> Handle(GetOverduePrescriptionsQuery request, CancellationToken ct)
    {
        // Validate pharmacy access
        var userPharmacyId = _user.GetPharmacyId();
        if (userPharmacyId.HasValue && userPharmacyId.Value != request.PharmacyId)
        {
            return Result<List<PrescriptionListItem>>.Forbidden();
        }

        var now = _dateTime.OffsetNow;

        var prescriptions = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId
                && p.ExpiryDate < now
                && p.Status != PrescriptionStatus.Delivered
                && p.Status != PrescriptionStatus.FullyDispensed
                && p.Status != PrescriptionStatus.Rejected
                && p.Status != PrescriptionStatus.Cancelled)
            .OrderBy(p => p.ExpiryDate)
            .Select(p => new PrescriptionListItem(
                p.Id,
                "RX-" + p.Id.ToString(),
                p.MainMember.FirstName + " " + p.MainMember.LastName,
                p.Beneficiary.FirstName + " " + p.Beneficiary.LastName,
                p.IssuedDate,
                p.ExpiryDate,
                p.Status.ToString(),
                p.Notes,
                true, // IsOverdue
                p.PrescriptionFiles != null ? p.PrescriptionFiles.Count : 0
            ))
            .ToListAsync(ct);

        return Result.Success(prescriptions);
    }
}
