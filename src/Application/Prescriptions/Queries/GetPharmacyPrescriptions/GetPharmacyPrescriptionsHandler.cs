using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPharmacyPrescriptions;

public class GetPharmacyPrescriptionsQueryHandler : IRequestHandler<GetPharmacyPrescriptionsQuery, Result<List<GetPharmacyPrescriptionsResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public GetPharmacyPrescriptionsQueryHandler(IApplicationDbContext db, IUser user)
    { _db = db; _user = user; }

    public async Task<Result<List<GetPharmacyPrescriptionsResult>>> Handle(GetPharmacyPrescriptionsQuery request, CancellationToken ct)
    {
        long? pharmacyId = request.PharmacyId;

        // fallback to claim for pharmacists
        if (pharmacyId is null)
        {
            // parse from claim stored as string
            // IUser only exposes Id; so we rely on controller to pass PharmacyId when needed or future IUser extension
        }

        var q = _db.Prescriptions
            .AsNoTracking()
            .Where(p => pharmacyId == null || p.AssignedPharmacyId == pharmacyId)
            .Select(p => new GetPharmacyPrescriptionsResult(
                p.Id,
                p.MainMember.Email!,
                p.AssignedPharmacyId,
                p.Status.ToString(),
                p.IssuedDate
            ));

        var list = await q.ToListAsync(ct);
        return Result.Success(list);
    }
}