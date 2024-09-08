using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
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

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new CreateUserRequest()
        {
            Email = request.Email,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role
        };
        var result = await _identityService.CreateUserAsync(user, cancellationToken);

        return result;
    }
}
