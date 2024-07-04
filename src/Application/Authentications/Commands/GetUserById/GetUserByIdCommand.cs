using ApiBaseTemplate.Application.Authentications.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Authentications.Commands.GetUserById;

public record GetUserByIdCommand(string Id) : IRequest<Result<UserDetailsResponse>>;

public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, Result<UserDetailsResponse>>
{
    private readonly IIdentityService _identityService;

    public GetUserByIdCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDetailsResponse>> Handle(GetUserByIdCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.GetUserById(request.Id, cancellationToken);
    }
}
