using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetRecentPrescriptionActivity;

public class GetRecentPrescriptionActivityHandler : IRequestHandler<GetRecentPrescriptionActivityQuery, Result<List<RecentActivityItem>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public GetRecentPrescriptionActivityHandler(IApplicationDbContext db, IUser user, IIdentityService identityService)
    {
        _db = db;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<List<RecentActivityItem>>> Handle(GetRecentPrescriptionActivityQuery request, CancellationToken ct)
    {
        // Get user's pharmacy context from JWT claims
        var userPharmacyId = _user.GetPharmacyId();

        // Check if user is SuperAdministrator (can access any pharmacy reports)
        var isSuperAdmin = await _identityService.IsInRoleAsync(_user.Id!, "SuperAdministrator", ct);

        // For non-SuperAdministrators, enforce pharmacy scope
        if (!isSuperAdmin.IsSuccess)
        {
            // Validate requested pharmacy matches user's pharmacy
            if (!userPharmacyId.HasValue || request.PharmacyId != userPharmacyId.Value)
            {
                return Result<List<RecentActivityItem>>.Forbidden();
            }
        }

        var q = _db.PrescriptionStatusHistories
            .AsNoTracking()
            .Where(h => _db.Prescriptions.Any(p => p.Id == h.PrescriptionId && p.AssignedPharmacyId == request.PharmacyId))
            .OrderByDescending(h => h.ChangedDate)
            .Select(h => new RecentActivityItem(h.PrescriptionId, h.OldStatus.ToString(), h.NewStatus.ToString(), h.ChangedDate, h.Reason))
            .Take(request.Limit);

        var list = await q.ToListAsync(ct);
        return Result.Success(list);
    }
}
