using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.CreateUserCommand;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;

    public CreateUserHandler(IIdentityService identityService)
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