using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Queries.ListAllPrescriptions;

public record ListAllPrescriptionsQuery(int Page = 1, int PageSize = 50) : IRequest<Result<List<ListPrescriptionItem>>>;

public record ListPrescriptionItem(long Id, string Status, string MemberEmail, long BeneficiaryId, string BeneficiaryName, DateTimeOffset IssuedDate);

