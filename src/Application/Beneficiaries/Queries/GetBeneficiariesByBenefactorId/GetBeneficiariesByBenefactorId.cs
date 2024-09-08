using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiariesByMainMemberId;

public record GetBeneficiariesByMainMemberIdCommand(string MainMemberId) : IRequest<Result<List<BeneficiaryResponse>>>;

public class GetBeneficiariesByMainMemberIdCommandValidator : AbstractValidator<GetBeneficiariesByMainMemberIdCommand>
{
    public GetBeneficiariesByMainMemberIdCommandValidator()
    {
    }
}

public class
    GetBeneficiariesByMainMemberIdCommandHandler : IRequestHandler<GetBeneficiariesByMainMemberIdCommand,
    Result<List<BeneficiaryResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetBeneficiariesByMainMemberIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BeneficiaryResponse>>> Handle(GetBeneficiariesByMainMemberIdCommand request,
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