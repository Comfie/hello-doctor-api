using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPrescriptionCounts;

public class GetPharmacyPrescriptionCountsHandler : IRequestHandler<GetPharmacyPrescriptionCountsQuery, Result<Dictionary<string,int>>>
{
    private readonly IApplicationDbContext _db;
    public GetPharmacyPrescriptionCountsHandler(IApplicationDbContext db) { _db = db; }

    public async Task<Result<Dictionary<string,int>>> Handle(GetPharmacyPrescriptionCountsQuery request, CancellationToken ct)
    {
        var groups = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId)
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        return Result.Success(groups.ToDictionary(x => x.Status, x => x.Count));
    }
}
