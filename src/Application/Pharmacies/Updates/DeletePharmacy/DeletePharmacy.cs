using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Repositories;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Pharmacies.Updates.DeletePharmacy;

public record DeletePharmacyCommand(long Id) : IRequest<Result<bool>>;

public class DeletePharmacyCommandValidator : AbstractValidator<DeletePharmacyCommand>
{
    public DeletePharmacyCommandValidator()
    {
    }
}

public class DeletePharmacyCommandHandler : IRequestHandler<DeletePharmacyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePharmacyCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _context
            .Pharmacies
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (pharmacy is null)
        {
            return Result.Failure<bool>(new Error("Delete Pharmacy", "Pharmacy not found"));
        }

        pharmacy.IsActive = false;
        pharmacy.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}