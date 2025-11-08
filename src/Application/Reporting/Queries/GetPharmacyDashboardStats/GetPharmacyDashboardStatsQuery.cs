using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyDashboardStats;

public record GetPharmacyDashboardStatsQuery(long PharmacyId) : IRequest<Result<PharmacyDashboardStatsResponse>>;
