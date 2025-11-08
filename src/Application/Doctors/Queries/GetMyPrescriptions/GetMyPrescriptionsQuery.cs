using Ardalis.Result;
using HelloDoctorApi.Application.Doctors.Models;

namespace HelloDoctorApi.Application.Doctors.Queries.GetMyPrescriptions;

public record GetMyPrescriptionsQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<List<DoctorPrescriptionResponse>>>;
