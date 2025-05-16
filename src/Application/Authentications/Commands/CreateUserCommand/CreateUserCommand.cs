using Ardalis.Result;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.CreateUserCommand;

public sealed record CreateUserCommand : IRequest<Result<bool>>
{
    public required string Email { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}