using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiaries;

public class
    GetBeneficiariesCommandHandler : IRequestHandler<GetBeneficiariesCommand, Result<List<BeneficiaryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public GetBeneficiariesCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<List<BeneficiaryResponse>>> Handle(GetBeneficiariesCommand request,
        CancellationToken cancellationToken)
    {
        // Check user role to determine filtering
        var isDoctor = await _identityService.IsInRoleAsync(_user.Id!, "Doctor", cancellationToken);
        var isSuperAdmin = await _identityService.IsInRoleAsync(_user.Id!, "SuperAdministrator", cancellationToken);

        var query = _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.MainMember)
            .Where(x => x.IsDeleted == false);

        // MainMembers only see their own beneficiaries
        // Doctors and SuperAdmins see all beneficiaries
        if (!isDoctor.IsSuccess && !isSuperAdmin.IsSuccess)
        {
            // MainMember - filter to only their beneficiaries
            if (string.IsNullOrWhiteSpace(_user.Id))
            {
                return Result.Success(new List<BeneficiaryResponse>());
            }
            query = query.Where(x => x.MainMemberId == _user.Id);
        }

        var beneficiaries = await query
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