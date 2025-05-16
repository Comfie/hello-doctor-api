using Ardalis.Result;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;

public record GetUserRolesCommand() : IRequest<Result<List<string?>>>;