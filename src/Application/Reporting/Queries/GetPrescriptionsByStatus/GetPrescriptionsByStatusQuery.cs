using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByStatus;

public record GetPrescriptionsByStatusQuery(
    long PharmacyId,
    PrescriptionStatus Status,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedPrescriptionsResponse>>;
