using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdCommand, Result<UserDetailsResponse>>
{
    private readonly IIdentityService _identityService;

    public GetUserByIdHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDetailsResponse>> Handle(GetUserByIdCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.GetUserById(request.Id, cancellationToken);
    }
}