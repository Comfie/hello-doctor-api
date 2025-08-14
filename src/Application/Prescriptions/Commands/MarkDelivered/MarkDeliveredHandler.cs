using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.MarkDelivered;

public class MarkDeliveredHandler : IRequestHandler<MarkDeliveredCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;

    public MarkDeliveredHandler(IApplicationDbContext db)
    { _db = db; }

    public async Task<Result<bool>> Handle(MarkDeliveredCommand request, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == request.PrescriptionId, ct);
        if (pres is null) return Result<bool>.NotFound();

        pres.MarkDelivered();
        await _db.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
