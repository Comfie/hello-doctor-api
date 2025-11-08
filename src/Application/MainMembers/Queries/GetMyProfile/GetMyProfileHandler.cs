using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.MainMembers.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.MainMembers.Queries.GetMyProfile;

public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, Result<MainMemberProfileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyProfileHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<MainMemberProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (string.IsNullOrWhiteSpace(userId))
            return Result<MainMemberProfileResponse>.Unauthorized();

        // Get user info
        var applicationUser = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (applicationUser == null)
            return Result<MainMemberProfileResponse>.NotFound();

        // Get MainMember data
        var mainMember = await _context.MainMembers
            .FirstOrDefaultAsync(m => m.AccountId == userId, cancellationToken);

        var response = new MainMemberProfileResponse
        {
            Id = mainMember?.Id ?? 0,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            EmailAddress = applicationUser.Email,
            PhoneNumber = applicationUser.PhoneNumber,
            Code = mainMember?.Code,
            DefaultPharmacyId = mainMember?.DefaultPharmacyId
        };

        return Result.Success(response);
    }
}
