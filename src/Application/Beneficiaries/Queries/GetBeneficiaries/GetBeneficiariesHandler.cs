using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiaries;

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
            .Include(beneficiary => beneficiary.MainMember)
            .Where(x => x.IsDeleted == false)
            .Select(beneficiary => new BeneficiaryResponse(
                beneficiary.Id,
                beneficiary.MainMember.FirstName + " " + beneficiary.MainMember.LastName,
                beneficiary.MainMember.PhoneNumber ?? string.Empty,
                beneficiary.FirstName,
                beneficiary.LastName,
                beneficiary.PhoneNumber,
                beneficiary.EmailAddress,
                beneficiary.MainMemberId,
                beneficiary.Relationship.ToString(),
                beneficiary.Gender,
                beneficiary.DateOfBirth,
                beneficiary.BeneficiaryCode
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(beneficiaries);
    }
}