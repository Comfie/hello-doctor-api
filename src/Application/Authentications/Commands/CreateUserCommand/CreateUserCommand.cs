using ApiBaseTemplate.Application.Authentications.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Authentications.Commands.CreateUserCommand;

public sealed record CreateUserCommand : IRequest<Result<bool>>
{
    public required string Email { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
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
            PhoneNumber = request.PhoneNumber
        };
        var result = await _identityService.CreateUserAsync(user, cancellationToken);

        return result;
    }
}
