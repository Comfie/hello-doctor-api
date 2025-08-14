namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPharmacyPrescriptions;

public record GetPharmacyPrescriptionsResult(long Id, string MemberEmail, long? AssignedPharmacyId, string Status, DateTimeOffset IssuedDate);
