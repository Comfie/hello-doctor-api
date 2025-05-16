using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.MainMembers.Queries.GetMainMembers;

public class GetMainMembersQueryHandler : IRequestHandler<GetMainMembersQuery, Result<List<MainMemberResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetMainMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MainMemberResponse>>> Handle(GetMainMembersQuery request,
        CancellationToken cancellationToken)
    {
        var mainMembers = await _context
            .MainMembers
            .Include(member => member.Account)
            .Include(member => member.Beneficiaries)
            .Select(mainMember => new MainMemberResponse
            {
                Id = mainMember.Id,
                MemberShipNumber = mainMember.Code,
                Email = mainMember.Account.Email,
                FirstName = mainMember.Account.FirstName,
                LastName = mainMember.Account.LastName,
                PhoneNumber = mainMember.Account.PhoneNumber,
                NumberOfBeneficiaries = mainMember.Beneficiaries != null ? mainMember.Beneficiaries.Count : 0
            })
            .ToListAsync(cancellationToken);

        return Result<List<MainMemberResponse>>.Success(mainMembers);
    }
}