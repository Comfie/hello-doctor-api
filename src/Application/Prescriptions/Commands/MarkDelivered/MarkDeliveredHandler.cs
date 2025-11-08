using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.MarkDelivered;

public class MarkDeliveredHandler : IRequestHandler<MarkDeliveredCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public MarkDeliveredHandler(IApplicationDbContext db, IUser user, IIdentityService identityService)
    {
        _db = db;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(MarkDeliveredCommand request, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == request.PrescriptionId, ct);
        if (pres is null) return Result<bool>.NotFound();

        // Get user's pharmacy context from JWT claims
        var userPharmacyId = _user.GetPharmacyId();

        // Check if user is SuperAdministrator (can mark any prescription as delivered)
        var isSuperAdmin = await _identityService.IsInRoleAsync(_user.Id!, "SuperAdministrator", ct);

        // For non-SuperAdministrators, enforce pharmacy scope
        if (!isSuperAdmin.IsSuccess)
        {
            // Validate prescription is assigned to user's pharmacy
            if (!userPharmacyId.HasValue || pres.AssignedPharmacyId != userPharmacyId.Value)
            {
                return Result<bool>.Forbidden();
            }
        }

        pres.MarkDelivered();
        await _db.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
