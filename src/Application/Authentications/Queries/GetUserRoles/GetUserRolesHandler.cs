using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;

public class GetUserRolesHandler : IRequestHandler<GetUserRolesCommand, Result<List<string>>>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user;

    public GetUserRolesHandler(IIdentityService identityService, IUser user)
    {
        _identityService = identityService;
        _user = user;
    }
    public async Task<Result<List<string>>> Handle(GetUserRolesCommand request, CancellationToken cancellationToken)
    {
        // Get pharmacy context for SystemAdministrators
        var pharmacyId = _user.GetPharmacyId();

        return await _identityService.GetUserRolesAsync(request.UserId, pharmacyId, cancellationToken);
    }
}