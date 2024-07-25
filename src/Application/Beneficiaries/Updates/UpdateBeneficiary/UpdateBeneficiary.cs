using ApiBaseTemplate.Application.Beneficiaries.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Repositories;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Beneficiaries.Updates.UpdateBeneficiary;

public record UpdateBeneficiaryCommand(
    long Id,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? EmailAddress) : IRequest<Result<BeneficiaryResponse>>;

public class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
    }
}

public class UpdateBeneficiaryCommandHandler : IRequestHandler<UpdateBeneficiaryCommand, Result<BeneficiaryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBeneficiaryCommandHandler(IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BeneficiaryResponse>> Handle(UpdateBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiary = await _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.Benefactor)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (beneficiary is null)
        {
            return Result.Failure<BeneficiaryResponse>(new Error("Beneficiary", "Beneficiary not found"));
        }

        beneficiary.FirstName = request.FirstName ?? beneficiary.FirstName;
        beneficiary.LastName = request.LastName ?? beneficiary.LastName;
        beneficiary.PhoneNumber = request.PhoneNumber ?? beneficiary.PhoneNumber;
        beneficiary.EmailAddress = request.EmailAddress ?? beneficiary.EmailAddress;
        _context.Beneficiaries.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new BeneficiaryResponse(
            beneficiary.Id,
            beneficiary.Benefactor.FirstName + " " + beneficiary.Benefactor.LastName,
            beneficiary.Benefactor.PhoneNumber ?? string.Empty,
            beneficiary.FirstName,
            beneficiary.LastName,
            beneficiary.PhoneNumber,
            beneficiary.EmailAddress,
            beneficiary.BenefactorId,
            beneficiary.Relationship.ToString()
        ));
    }
}