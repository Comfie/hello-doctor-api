using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Authentications.Commands.GetAllUsers;

public record GetAllUsersCommand : IRequest<Result<List<UserDetailsResponse>>>;