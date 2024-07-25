using ApiBaseTemplate.Application.Beneficiaries.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Beneficiaries.Queries.GetBeneficiaries;

public record GetBeneficiariesCommand : IRequest<Result<List<BeneficiaryResponse>>>
{
}

public class GetBeneficiariesCommandValidator : AbstractValidator<GetBeneficiariesCommand>
{
    public GetBeneficiariesCommandValidator()
    {
    }
}

public class
    GetBeneficiariesCommandHandler : IRequestHandler<GetBeneficiariesCommand, Result<List<BeneficiaryResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetBeneficiariesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BeneficiaryResponse>>> Handle(GetBeneficiariesCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiaries = await _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.Benefactor)
            .Where(x => x.IsDeleted == false)
            .Select(beneficiary => new BeneficiaryResponse(
                beneficiary.Id,
                beneficiary.Benefactor.FirstName + " " + beneficiary.Benefactor.LastName,
                beneficiary.Benefactor.PhoneNumber ?? string.Empty,
                beneficiary.FirstName,
                beneficiary.LastName,
                beneficiary.PhoneNumber,
                beneficiary.EmailAddress,
                beneficiary.BenefactorId,
                beneficiary.Relationship.ToString()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(beneficiaries);
    }
}