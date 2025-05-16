using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersCommand, Result<List<UserDetailsResponse>>>
{
    private readonly IIdentityService _identityService;

    public GetAllUsersHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<List<UserDetailsResponse>>> Handle(GetAllUsersCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.GetUsers(cancellationToken);
    }
}