using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Authentications.Updates.RevokeUserRole;

public class RevokeUserRoleHandler : IRequestHandler<RevokeUserRoleCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;

    public RevokeUserRoleHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<bool>> Handle(RevokeUserRoleCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RevokeRoleAsync(request.UserId, request.Role, cancellationToken);
    }
}