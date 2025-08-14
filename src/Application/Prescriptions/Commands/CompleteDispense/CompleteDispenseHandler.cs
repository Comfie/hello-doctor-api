using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.CompleteDispense;

public class CompleteDispenseHandler : IRequestHandler<CompleteDispenseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;

    public CompleteDispenseHandler(IApplicationDbContext db)
    { _db = db; }

    public async Task<Result<bool>> Handle(CompleteDispenseCommand request, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == request.PrescriptionId, ct);
        if (pres is null) return Result<bool>.NotFound();

        pres.CompleteDispense(request.IsPartial, request.Note);
        await _db.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
