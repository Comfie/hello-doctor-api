using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPrescriptionDetails;

public class GetPrescriptionDetailsHandler : IRequestHandler<GetPrescriptionDetailsQuery, Result<PrescriptionDetailsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPrescriptionDetailsHandler(IApplicationDbContext db) { _db = db; }

    public async Task<Result<PrescriptionDetailsDto>> Handle(GetPrescriptionDetailsQuery request, CancellationToken ct)
    {
        var p = await _db.Prescriptions
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.MainMember)
            .Include(x => x.PrescriptionFiles!)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (p is null) return Result<PrescriptionDetailsDto>.NotFound();

        var dto = new PrescriptionDetailsDto(
            p.Id,
            p.Status.ToString(),
            p.MainMember.Email ?? string.Empty,
            p.BeneficiaryId,
            $"{p.Beneficiary.FirstName} {p.Beneficiary.LastName}",
            p.IssuedDate,
            p.Notes,
            p.AssignedPharmacyId,
            p.PrescriptionFiles?.Select(f => f.Id).ToList() ?? new List<long>()
        );
        return Result.Success(dto);
    }
}

