namespace HelloDoctorApi.Application.Doctors.Models;

public record DoctorPrescriptionResponse(
    long Id,
    string PrescriptionCode,
    string MainMemberName,
    string BeneficiaryName,
    DateTimeOffset IssuedDate,
    DateTimeOffset ExpiryDate,
    string Status,
    string? AssignedPharmacyName,
    string? Notes,
    bool IsOverdue,
    DateTimeOffset CreatedAt
);
