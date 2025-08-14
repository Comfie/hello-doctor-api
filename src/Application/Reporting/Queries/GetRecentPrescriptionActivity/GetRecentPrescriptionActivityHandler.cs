using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetRecentPrescriptionActivity;

public class GetRecentPrescriptionActivityHandler : IRequestHandler<GetRecentPrescriptionActivityQuery, Result<List<RecentActivityItem>>>
{
    private readonly IApplicationDbContext _db;
    public GetRecentPrescriptionActivityHandler(IApplicationDbContext db) { _db = db; }

    public async Task<Result<List<RecentActivityItem>>> Handle(GetRecentPrescriptionActivityQuery request, CancellationToken ct)
    {
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
