using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.RefreshToken;

public sealed record RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}