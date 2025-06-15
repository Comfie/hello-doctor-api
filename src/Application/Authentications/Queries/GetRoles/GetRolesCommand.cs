using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;

namespace HelloDoctorApi.Application.Authentications.Queries.GetRoles;

public record GetRolesCommand() : IRequest<Result<List<UserRoleResponse>>>;