namespace HelloDoctorApi.Application.Payments.Models;

public record PaymentInitiationResponse(
    bool Success,
    string? PaymentUrl,
    string? ExternalReference,
    string? ErrorMessage = null
);
