using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Repositories;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Beneficiaries.Updates.DeleteBeneficiary;

public record DeleteBeneficiaryCommand(long Id) : IRequest<Result<bool>>
{
}

public class DeleteBeneficiaryCommandValidator : AbstractValidator<DeleteBeneficiaryCommand>
{
    public DeleteBeneficiaryCommandValidator()
    {
    }
}

public class DeleteBeneficiaryCommandHandler : IRequestHandler<DeleteBeneficiaryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBeneficiaryCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var beneficiary = await _context
            .Beneficiaries
            .SingleAsync(x => x.Id == request.Id, cancellationToken);
        
        beneficiary.IsDeleted = true;
        _context.Beneficiaries.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(true);
    }
}
