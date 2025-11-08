using Ardalis.Result;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Doctors.Queries.GetMyProfile;

public record GetMyProfileQuery() : IRequest<Result<DoctorResponse>>;
