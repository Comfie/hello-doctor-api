using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetMyPrescriptions;

public record GetMyPrescriptionsQuery(string? BeneficiaryCode = null)
  : IRequest<Result<List<MyPrescriptionItem>>>;

public record MyPrescriptionItem(
  long Id, 
  string Status,
  DateTimeOffset IssuedDate, 
  long BeneficiaryId,
  string? Notes, 
  DateTimeOffset ExpiryDate,
  int FileCount,
  string BeneficiaryCode);

