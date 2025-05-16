using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Updates.UpdateUser;

public sealed record UpdateUserCommand(
    string Id,
    UpdateUserRequest Request) : IRequest<Result<UserDetailsResponse>>;