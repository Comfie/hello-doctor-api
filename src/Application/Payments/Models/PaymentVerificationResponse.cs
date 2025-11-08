using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Payments.Models;

public record PaymentVerificationResponse(
    bool Success,
    PaymentStatus Status,
    string? ExternalTransactionId,
    decimal? AmountPaid,
    string? ErrorMessage = null
);
