using Ardalis.Result;

namespace HelloDoctorApi.Application.Reporting.Queries.GetMyPrescriptionCounts;

public record GetMyPrescriptionCountsQuery() : IRequest<Result<Dictionary<string,int>>>;

