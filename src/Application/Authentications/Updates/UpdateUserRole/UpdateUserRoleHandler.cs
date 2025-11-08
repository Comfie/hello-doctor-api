using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Updates.UpdateUserRole;

public class UpdateUserRoleHandler : IRequestHandler<UpdateUserRoleCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user;

    public UpdateUserRoleHandler(IIdentityService identityService, IUser user)
    {
        _identityService = identityService;
        _user = user;
    }

    public async Task<Result<bool>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        // Get pharmacy context for SystemAdministrators
        var pharmacyId = _user.GetPharmacyId();

        return await _identityService.UpdateRoleAsync(request.UserId, request.Role, pharmacyId, cancellationToken);
    }
}