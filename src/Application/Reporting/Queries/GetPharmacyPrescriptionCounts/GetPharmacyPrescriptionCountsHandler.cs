using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPrescriptionCounts;

public class GetPharmacyPrescriptionCountsHandler : IRequestHandler<GetPharmacyPrescriptionCountsQuery, Result<Dictionary<string,int>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public GetPharmacyPrescriptionCountsHandler(IApplicationDbContext db, IUser user, IIdentityService identityService)
    {
        _db = db;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<Dictionary<string,int>>> Handle(GetPharmacyPrescriptionCountsQuery request, CancellationToken ct)
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
                return Result<Dictionary<string, int>>.Forbidden();
            }
        }

        var groups = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId)
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        return Result.Success(groups.ToDictionary(x => x.Status, x => x.Count));
    }
}
