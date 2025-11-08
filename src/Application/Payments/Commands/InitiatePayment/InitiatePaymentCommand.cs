using Ardalis.Result;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Payments.Commands.InitiatePayment;

public record InitiatePaymentCommand(
    decimal Amount,
    PaymentPurpose Purpose,
    PaymentProvider Provider,
    long? PrescriptionId = null,
    string? PayeeId = null,
    string? PayeeType = null,
    string? Notes = null
) : IRequest<Result<InitiatePaymentResponse>>;

public record InitiatePaymentResponse(
    long PaymentId,
    string PaymentUrl,
    string Status
);
