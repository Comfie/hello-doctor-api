using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.MainMembers.Commands.UpdateMyProfile;

public class UpdateMyProfileHandler : IRequestHandler<UpdateMyProfileCommand, Result<MainMemberProfileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMyProfileHandler(IApplicationDbContext context, IUser user, IUnitOfWork unitOfWork)
    {
        _context = context;
        _user = user;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MainMemberProfileResponse>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (string.IsNullOrWhiteSpace(userId))
            return Result<MainMemberProfileResponse>.Unauthorized();

        // Get and update user info
        var applicationUser = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (applicationUser == null)
            return Result<MainMemberProfileResponse>.NotFound();

        // Update ApplicationUser fields
        if (!string.IsNullOrWhiteSpace(request.FirstName))
            applicationUser.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            applicationUser.LastName = request.LastName;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            applicationUser.PhoneNumber = request.PhoneNumber;

        // Update MainMember
        var mainMember = await _context.MainMembers
            .FirstOrDefaultAsync(m => m.AccountId == userId, cancellationToken);

        if (mainMember != null)
        {
            mainMember.DefaultPharmacyId = request.DefaultPharmacyId;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
