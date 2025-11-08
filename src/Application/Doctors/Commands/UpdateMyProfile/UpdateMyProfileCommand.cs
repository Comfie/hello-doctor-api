using Ardalis.Result;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Doctors.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(UpdateDoctorProfileRequest Request) : IRequest<Result<DoctorResponse>>;
