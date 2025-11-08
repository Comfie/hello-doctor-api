using Ardalis.Result;

namespace HelloDoctorApi.Application.Payments.Queries.GetPaymentHistory;

public record GetPaymentHistoryQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<List<PaymentHistoryItem>>>;

public record PaymentHistoryItem(
    long PaymentId,
    string Status,
    decimal Amount,
    string Currency,
    string Purpose,
    string Provider,
    long? PrescriptionId,
    DateTimeOffset InitiatedAt,
    DateTimeOffset? CompletedAt
);
