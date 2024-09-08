using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    string Id,
    UpdateUserRequest Request) : IRequest<Result<UserDetailsResponse>>;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDetailsResponse>>
{
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task<Result<UserDetailsResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.UpdateUserAsync(request.Id, request.Request, cancellationToken);
    }
}
