using Ardalis.Result;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPrescriptionCounts;

public record GetPharmacyPrescriptionCountsQuery(long PharmacyId) : IRequest<Result<Dictionary<string,int>>>;
