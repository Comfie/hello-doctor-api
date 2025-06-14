using Ardalis.Result;

namespace HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;

public sealed record GetUserRolesCommand(string UserId) : IRequest<Result<List<string>>>;