using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.GetUserById;

public record GetUserByIdCommand(string Id) : IRequest<Result<UserDetailsResponse>>;