using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Payments.Models;

public record PaymentCallbackResponse(
    bool Success,
    long PaymentId,
    PaymentStatus Status,
    string? ExternalTransactionId,
    decimal? AmountPaid,
    string? ErrorMessage = null
);
