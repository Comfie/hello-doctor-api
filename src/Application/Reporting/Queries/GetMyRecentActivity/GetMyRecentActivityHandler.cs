using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetMyRecentActivity;

public class GetMyRecentActivityHandler : IRequestHandler<GetMyRecentActivityQuery, Result<List<MyRecentActivityItem>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public GetMyRecentActivityHandler(IApplicationDbContext db, IUser user) { _db = db; _user = user; }

    public async Task<Result<List<MyRecentActivityItem>>> Handle(GetMyRecentActivityQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return Result<List<MyRecentActivityItem>>.Unauthorized();

        var memberPrescriptionIds = _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.MainMemberId == _user.Id)
            .Select(p => p.Id);

        var q = _db.PrescriptionStatusHistories
            .AsNoTracking()
            .Where(h => memberPrescriptionIds.Contains(h.PrescriptionId))
            .OrderByDescending(h => h.ChangedDate)
            .Select(h => new MyRecentActivityItem(h.PrescriptionId, h.OldStatus.ToString(), h.NewStatus.ToString(), h.ChangedDate, h.Reason))
            .Take(request.Limit);

        var list = await q.ToListAsync(ct);
        return Result.Success(list);
    }
}

