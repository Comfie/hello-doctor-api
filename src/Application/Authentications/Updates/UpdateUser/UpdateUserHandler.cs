using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Updates.UpdateUser;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<UserDetailsResponse>>
{
    private readonly IIdentityService _identityService;

    public UpdateUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDetailsResponse>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.UpdateUserAsync(request.Id, request.Request, cancellationToken);
    }
}