using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.MainMembers.Queries.GetMainMemberById;

public record GetMainMemberByIdQuery(long Id) : IRequest<Result<MainMemberResponse>>
{
}

public class GetMainMemberByIdQueryValidator : AbstractValidator<GetMainMemberByIdQuery>
{
    public GetMainMemberByIdQueryValidator()
    {
    }
}

public class GetMainMemberByIdQueryHandler : IRequestHandler<GetMainMemberByIdQuery, Result<MainMemberResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetMainMemberByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MainMemberResponse>> Handle(GetMainMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var mainMember = await _context
            .MainMembers
            .Include(member => member.Account)
            .Include(member => member.Beneficiaries)
            .Where(mainMember => mainMember.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
        
        if (mainMember is null)
        {
            return Result.Failure<MainMemberResponse>(new Error("Get MaincMember", "Main member not found"));
        }

        return Result.Success(mainMember); 
    }
}
