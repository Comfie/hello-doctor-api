using Ardalis.Result;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Updates.UpdateUserRole;

public record UpdateUserRoleCommand(string UserId, string Role) : IRequest<Result<bool>>;