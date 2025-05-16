using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.AuthenticateUser;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public AuthenticateUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.AuthenticateUserAsync(request.Email, request.Password, cancellationToken);
    }
}