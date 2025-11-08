using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Reporting.Models;

public record PrescriptionListItem(
    long Id,
    string PrescriptionCode,
    string MainMemberName,
    string BeneficiaryName,
    DateTimeOffset IssuedDate,
    DateTimeOffset ExpiryDate,
    string Status,
    string? Notes,
    bool IsOverdue,
    int FileCount
);
