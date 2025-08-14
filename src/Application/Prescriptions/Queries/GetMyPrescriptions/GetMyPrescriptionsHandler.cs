using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetMyPrescriptions;

public class GetMyPrescriptionsHandler : IRequestHandler<GetMyPrescriptionsQuery, Result<List<MyPrescriptionItem>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public GetMyPrescriptionsHandler(IApplicationDbContext db, IUser user)
    { _db = db; _user = user; }

    public async Task<Result<List<MyPrescriptionItem>>> Handle(GetMyPrescriptionsQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return Result<List<MyPrescriptionItem>>.Unauthorized();

        var q = _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.MainMemberId == _user.Id)
            .Where(p => string.IsNullOrEmpty(request.BeneficiaryCode) || p.Beneficiary.BeneficiaryCode == request.BeneficiaryCode)
            .Select(p => new MyPrescriptionItem(p.Id, p.Status.ToString(), p.IssuedDate, p.BeneficiaryId, p.Notes));

        var list = await q.ToListAsync(ct);
        return Result.Success(list);
    }
}

