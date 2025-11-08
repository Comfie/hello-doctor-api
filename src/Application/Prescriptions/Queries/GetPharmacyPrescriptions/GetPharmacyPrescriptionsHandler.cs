using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPharmacyPrescriptions;

public class GetPharmacyPrescriptionsQueryHandler : IRequestHandler<GetPharmacyPrescriptionsQuery, Result<List<GetPharmacyPrescriptionsResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public GetPharmacyPrescriptionsQueryHandler(IApplicationDbContext db, IUser user, IIdentityService identityService)
    {
        _db = db;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<List<GetPharmacyPrescriptionsResult>>> Handle(GetPharmacyPrescriptionsQuery request, CancellationToken ct)
    {
        // Get user's pharmacy context from JWT claims
        var userPharmacyId = _user.GetPharmacyId();

        // Check if user is SuperAdministrator (can access any pharmacy)
        var isSuperAdmin = await _identityService.IsInRoleAsync(_user.Id!, "SuperAdministrator", ct);

        // Determine which pharmacyId to use for filtering
        long? pharmacyId = request.PharmacyId;

        // For non-SuperAdministrators, enforce pharmacy scope
        if (!isSuperAdmin.IsSuccess)
        {
            // If no pharmacyId in request, use user's pharmacy from claims
            if (pharmacyId is null)
            {
                pharmacyId = userPharmacyId;
            }
            // If pharmacyId is provided, validate it matches user's pharmacy
            else if (userPharmacyId.HasValue && pharmacyId != userPharmacyId)
            {
                return Result<List<GetPharmacyPrescriptionsResult>>.Forbidden();
            }

            // If user has no pharmacy context, they cannot access prescriptions
            if (pharmacyId is null)
            {
                return Result<List<GetPharmacyPrescriptionsResult>>.Forbidden();
            }
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