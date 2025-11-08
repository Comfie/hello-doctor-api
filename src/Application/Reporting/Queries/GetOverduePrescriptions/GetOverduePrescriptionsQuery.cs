using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;

namespace HelloDoctorApi.Application.Reporting.Queries.GetOverduePrescriptions;

public record GetOverduePrescriptionsQuery(long PharmacyId) : IRequest<Result<List<PrescriptionListItem>>>;
