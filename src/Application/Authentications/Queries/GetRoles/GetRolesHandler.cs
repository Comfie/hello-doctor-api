using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Authentications.Queries.GetRoles;

public class GetRolesHandler : IRequestHandler<GetRolesCommand, Result<List<UserRoleResponse>>>
{
    private readonly IIdentityService _identityService;

    public GetRolesHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<List<UserRoleResponse>>> Handle(GetRolesCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.GetRolesAsync(cancellationToken);
    }
}