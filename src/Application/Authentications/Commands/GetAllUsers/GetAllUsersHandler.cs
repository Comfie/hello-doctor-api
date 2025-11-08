using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersCommand, Result<List<UserDetailsResponse>>>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user;

    public GetAllUsersHandler(IIdentityService identityService, IUser user)
    {
        _identityService = identityService;
        _user = user;
    }

    public async Task<Result<List<UserDetailsResponse>>> Handle(GetAllUsersCommand request,
        CancellationToken cancellationToken)
    {
        // Get pharmacy context for SystemAdministrators
        // SuperAdministrators will have null pharmacyId (see all users)
        var pharmacyId = _user.GetPharmacyId();

        return await _identityService.GetUsers(pharmacyId, cancellationToken);
    }
}