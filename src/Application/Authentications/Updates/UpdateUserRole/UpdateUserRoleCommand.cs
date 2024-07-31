using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Authentications.Updates.UpdateUserRole;

public record UpdateUserRoleCommand(string UserId, string Role) : IRequest<Result<bool>>;


public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;

    public UpdateUserRoleCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.UpdateRoleAsync(request.UserId, request.Role, cancellationToken);
    }
}
