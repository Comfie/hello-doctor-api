using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Authentications.Commands.CreateRole;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<bool>>
{
    
    private readonly IIdentityService _identityService;

    public CreateRoleHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<bool>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.CreateRoleAsync(request.Name, cancellationToken);
    }
}