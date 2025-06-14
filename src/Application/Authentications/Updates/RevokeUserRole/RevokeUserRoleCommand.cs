using Ardalis.Result;

namespace HelloDoctorApi.Application.Authentications.Updates.RevokeUserRole;

public sealed record RevokeUserRoleCommand(string UserId, string Role) : IRequest<Result<bool>>;