using ApiBaseTemplate.Application.Authentications.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Authentications.Commands.AuthenticateUser;

public class AuthenticateUserCommand : IRequest<Result<AuthResponse>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    public AuthenticateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task<Result<AuthResponse>>  Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.AuthenticateUserAsync(request.Email, request.Password, cancellationToken);
    }
}
