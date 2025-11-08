using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Doctors.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Doctors.Queries.GetMyPrescriptions;

public class GetMyPrescriptionsHandler : IRequestHandler<GetMyPrescriptionsQuery, Result<List<DoctorPrescriptionResponse>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IDateTimeService _dateTime;

    public GetMyPrescriptionsHandler(IApplicationDbContext db, IUser user, IDateTimeService dateTime)
    {
        _db = db;
        _user = user;
        _dateTime = dateTime;
    }

    public async Task<Result<List<DoctorPrescriptionResponse>>> Handle(GetMyPrescriptionsQuery request, CancellationToken ct)
    {
        // Verify doctor is authenticated
        if (string.IsNullOrWhiteSpace(_user.Id))
            return Result<List<DoctorPrescriptionResponse>>.Unauthorized();

        var doctorId = _user.GetDoctorId();
        if (!doctorId.HasValue)
            return Result<List<DoctorPrescriptionResponse>>.Forbidden();

        var now = _dateTime.OffsetNow;

        var prescriptions = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.DoctorId == doctorId.Value)
            .OrderByDescending(p => p.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new DoctorPrescriptionResponse(
                p.Id,
                "RX-" + p.Id.ToString(),
                p.MainMember.FirstName + " " + p.MainMember.LastName,
                p.Beneficiary.FirstName + " " + p.Beneficiary.LastName,
                p.IssuedDate,
                p.ExpiryDate,
                p.Status.ToString(),
                p.AssignedPharmacy != null ? p.AssignedPharmacy.Name : null,
                p.Notes,
                p.ExpiryDate < now,
                p.Created
            ))
            .ToListAsync(ct);

        return Result.Success(prescriptions);
    }
}
