using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiary;

public class GetBeneficiaryCommandHandler : IRequestHandler<GetBeneficiaryCommand, Result<BeneficiaryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetBeneficiaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BeneficiaryResponse>> Handle(GetBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiary = await _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.MainMember)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return beneficiary is null
            ? Result<BeneficiaryResponse>.NotFound(new Error("Beneficiary", "Not found"))
            : Result.Success(beneficiary);
    }
}