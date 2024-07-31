using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Authentications.Queries.GetUserRoles;

public record GetUserRolesCommand() : IRequest<Result<List<string?>>>;

public class GetUserRolesCommandHandler : IRequestHandler<GetUserRolesCommand, Result<List<string?>>>
{
    private readonly IIdentityService _identityService;

    public GetUserRolesCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<List<string?>>> Handle(GetUserRolesCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.GetRolesAsync(cancellationToken);
    }
}
