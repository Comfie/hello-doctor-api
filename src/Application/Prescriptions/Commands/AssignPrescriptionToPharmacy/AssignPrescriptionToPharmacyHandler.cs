using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.AssignPrescriptionToPharmacy;

public class AssignPrescriptionToPharmacyHandler : IRequestHandler<AssignPrescriptionToPharmacyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;

    public AssignPrescriptionToPharmacyHandler(IApplicationDbContext db)
    { _db = db; }

    public async Task<Result<bool>> Handle(AssignPrescriptionToPharmacyCommand request, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == request.PrescriptionId, ct);
        if (pres is null) return Result<bool>.NotFound();

        pres.AssignToPharmacy(request.PharmacyId);
        await _db.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
