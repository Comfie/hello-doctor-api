using Ardalis.Result;

namespace HelloDoctorApi.Application.Authentications.Updates.ChangeRoleStatus;

public sealed record ChangeRoleStatusCommand(string Role, bool Status) : IRequest<Result<bool>>;