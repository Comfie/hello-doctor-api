using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.AssignPrescriptionToPharmacy;

public class AssignPrescriptionToPharmacyHandler : IRequestHandler<AssignPrescriptionToPharmacyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AssignPrescriptionToPharmacyHandler(IApplicationDbContext db, IUser user, IIdentityService identityService)
    {
        _db = db;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(AssignPrescriptionToPharmacyCommand request, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == request.PrescriptionId, ct);
        if (pres is null) return Result<bool>.NotFound();

        // Get user's pharmacy context from JWT claims
        var userPharmacyId = _user.GetPharmacyId();

        // Check if user is SuperAdministrator (can assign to any pharmacy)
        var isSuperAdmin = await _identityService.IsInRoleAsync(_user.Id!, "SuperAdministrator", ct);

        // For non-SuperAdministrators, enforce pharmacy scope
        if (!isSuperAdmin.IsSuccess)
        {
            // Validate user can only assign to their own pharmacy
            if (!userPharmacyId.HasValue || request.PharmacyId != userPharmacyId.Value)
            {
                return Result<bool>.Forbidden();
            }
        }

        pres.AssignToPharmacy(request.PharmacyId);
        await _db.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
