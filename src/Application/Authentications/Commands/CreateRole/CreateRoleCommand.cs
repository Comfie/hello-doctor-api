using Ardalis.Result;

namespace HelloDoctorApi.Application.Authentications.Commands.CreateRole;

public sealed record CreateRoleCommand(string Name) : IRequest<Result<bool>>;