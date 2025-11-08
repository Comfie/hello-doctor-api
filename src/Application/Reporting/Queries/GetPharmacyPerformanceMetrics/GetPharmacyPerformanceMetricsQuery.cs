using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPerformanceMetrics;

public record GetPharmacyPerformanceMetricsQuery(long PharmacyId, int DaysBack = 30) : IRequest<Result<PerformanceMetricsResponse>>;
