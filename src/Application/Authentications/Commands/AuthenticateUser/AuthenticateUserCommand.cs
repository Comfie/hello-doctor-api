using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;

namespace HelloDoctorApi.Application.Authentications.Commands.AuthenticateUser;

public class AuthenticateUserCommand : IRequest<Result<AuthResponse>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}