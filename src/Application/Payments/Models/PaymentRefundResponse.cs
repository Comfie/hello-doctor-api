namespace HelloDoctorApi.Application.Payments.Models;

public record PaymentRefundResponse(
    bool Success,
    string? RefundTransactionId,
    decimal? RefundedAmount,
    string? ErrorMessage = null
);
