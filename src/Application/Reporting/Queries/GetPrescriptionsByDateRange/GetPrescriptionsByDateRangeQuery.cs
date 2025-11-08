using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByDateRange;

public record GetPrescriptionsByDateRangeQuery(
    long PharmacyId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedPrescriptionsResponse>>;
