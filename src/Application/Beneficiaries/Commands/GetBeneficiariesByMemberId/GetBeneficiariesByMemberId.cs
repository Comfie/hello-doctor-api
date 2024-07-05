using ApiBaseTemplate.Application.Beneficiaries.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Beneficiaries.Commands.GetBeneficiariesByMemberId;

public record GetBeneficiariesByMemberIdCommand(string MainMemberId) : IRequest<Result<List<BeneficiaryResponse>>>;

public class GetBeneficiariesByMemberIdCommandValidator : AbstractValidator<GetBeneficiariesByMemberIdCommand>
{
    public GetBeneficiariesByMemberIdCommandValidator()
    {
    }
}

public class
    GetBeneficiariesByMemberIdCommandHandler : IRequestHandler<GetBeneficiariesByMemberIdCommand,
    Result<List<BeneficiaryResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetBeneficiariesByMemberIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BeneficiaryResponse>>> Handle(GetBeneficiariesByMemberIdCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiaries = await _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.MainMember)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.MainMemberId == request.MainMemberId)
            .Select(beneficiary => new BeneficiaryResponse(
                beneficiary.Id,
                beneficiary.MainMember.FirstName + " " + beneficiary.MainMember.LastName,
                beneficiary.MainMember.PhoneNumber ?? string.Empty,
                beneficiary.FirstName,
                beneficiary.LastName,
                beneficiary.PhoneNumber,
                beneficiary.EmailAddress,
                beneficiary.MainMemberId,
                beneficiary.Relationship.ToString()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(beneficiaries);
    }
}