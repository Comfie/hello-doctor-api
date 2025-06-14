using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Authentications.Updates.ChangeRoleStatus;

public class ChangeRoleStatusHandler : IRequestHandler<ChangeRoleStatusCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;

    public ChangeRoleStatusHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<bool>> Handle(ChangeRoleStatusCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.UpdateRoleStatusAsync(request.Role, cancellationToken);
    }
}