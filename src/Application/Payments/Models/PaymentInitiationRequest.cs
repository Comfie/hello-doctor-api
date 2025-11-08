using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Payments.Models;

public record PaymentInitiationRequest(
    long PaymentId,
    decimal Amount,
    string Currency,
    PaymentPurpose Purpose,
    long? PrescriptionId,
    string PayerEmail,
    string PayerName,
    string ReturnUrl,
    string CancelUrl,
    string NotifyUrl
);
