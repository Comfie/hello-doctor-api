using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Queries.ListAllPrescriptions;

public class ListAllPrescriptionsHandler : IRequestHandler<ListAllPrescriptionsQuery, Result<List<ListPrescriptionItem>>>
{
    private readonly IApplicationDbContext _db;

    public ListAllPrescriptionsHandler(IApplicationDbContext db) { _db = db; }

    public async Task<Result<List<ListPrescriptionItem>>> Handle(ListAllPrescriptionsQuery request, CancellationToken ct)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var query = _db.Prescriptions
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.MainMember)
            .OrderByDescending(p => p.IssuedDate)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(p => new ListPrescriptionItem(
                p.Id,
                p.Status.ToString(),
                p.MainMember.Email ?? string.Empty,
                p.BeneficiaryId,
                p.Beneficiary.FirstName + " " + p.Beneficiary.LastName,
                p.IssuedDate
            ));

        var list = await query.ToListAsync(ct);
        return Result.Success(list);
    }
}

