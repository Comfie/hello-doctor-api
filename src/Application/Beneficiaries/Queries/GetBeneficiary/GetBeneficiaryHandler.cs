using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiary;

public class GetBeneficiaryCommandHandler : IRequestHandler<GetBeneficiaryCommand, Result<BeneficiaryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public GetBeneficiaryCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<BeneficiaryResponse>> Handle(GetBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        // Validate user is authenticated
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            return Result<BeneficiaryResponse>.Forbidden();
        }

        // Check user role to determine filtering
        var isDoctor = await _identityService.IsInRoleAsync(_user.Id, "Doctor", cancellationToken);
        var isSuperAdmin = await _identityService.IsInRoleAsync(_user.Id, "SuperAdministrator", cancellationToken);

        var query = _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.MainMember)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.Id == request.Id);

        // MainMembers only see their own beneficiaries
        // Doctors and SuperAdmins can see any beneficiary
        if (!isDoctor.IsSuccess && !isSuperAdmin.IsSuccess)
        {
            query = query.Where(x => x.MainMemberId == _user.Id); // Enforce ownership for MainMembers
        }

        var beneficiary = await query
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
            .FirstOrDefaultAsync(cancellationToken);

        return beneficiary is null
            ? Result<BeneficiaryResponse>.NotFound(new Error("Beneficiary", "Not found"))
            : Result.Success(beneficiary);
    }
}