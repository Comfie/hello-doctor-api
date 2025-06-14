using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;

public class GetUserRolesHandler : IRequestHandler<GetUserRolesCommand, Result<List<string>>>
{
    private readonly IIdentityService _identityService;

    public GetUserRolesHandler(IIdentityService identityService)    
    {
        _identityService = identityService;
    }
    public async Task<Result<List<string>>> Handle(GetUserRolesCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.GetUserRolesAsync(request.UserId, cancellationToken);
    }
}