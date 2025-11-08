using Ardalis.Result;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Doctors.Queries.GetAllDoctors;

public record GetAllDoctorsQuery() : IRequest<Result<List<DoctorResponse>>>;
