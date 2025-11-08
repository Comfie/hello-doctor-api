using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;

namespace HelloDoctorApi.Application.Reporting.Queries.GetTodaysPrescriptions;

public record GetTodaysPrescriptionsQuery(long PharmacyId) : IRequest<Result<List<PrescriptionListItem>>>;
