using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetMyPrescriptionCounts;

public class GetMyPrescriptionCountsHandler : IRequestHandler<GetMyPrescriptionCountsQuery, Result<Dictionary<string,int>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public GetMyPrescriptionCountsHandler(IApplicationDbContext db, IUser user) { _db = db; _user = user; }

    public async Task<Result<Dictionary<string, int>>> Handle(GetMyPrescriptionCountsQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return Result<Dictionary<string,int>>.Unauthorized();

        var groups = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.MainMemberId == _user.Id)
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        return Result.Success(groups.ToDictionary(x => x.Status, x => x.Count));
    }
}

