using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.GetUserById;

public record GetUserByIdCommand(string Id) : IRequest<Result<UserDetailsResponse>>;

public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, Result<UserDetailsResponse>>
{
    private readonly IIdentityService _identityService;

    public GetUserByIdCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDetailsResponse>> Handle(GetUserByIdCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.GetUserById(request.Id, cancellationToken);
    }
}
