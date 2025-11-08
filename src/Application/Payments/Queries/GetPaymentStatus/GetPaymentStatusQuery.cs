using Ardalis.Result;

namespace HelloDoctorApi.Application.Payments.Queries.GetPaymentStatus;

public record GetPaymentStatusQuery(long PaymentId) : IRequest<Result<PaymentStatusResponse>>;

public record PaymentStatusResponse(
    long PaymentId,
    string Status,
    decimal Amount,
    string Currency,
    string Purpose,
    string? ExternalTransactionId,
    DateTimeOffset? CompletedAt,
    string? FailureReason
);
