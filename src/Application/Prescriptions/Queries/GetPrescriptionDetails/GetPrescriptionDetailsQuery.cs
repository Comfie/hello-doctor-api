using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPrescriptionDetails;

public record GetPrescriptionDetailsQuery(long Id) : IRequest<Result<PrescriptionDetailsDto>>;

public record PrescriptionDetailsDto(long Id, string Status, string MemberEmail, long BeneficiaryId, string BeneficiaryName, DateTimeOffset IssuedDate, string? Notes, long? AssignedPharmacyId, List<long> FileIds);

