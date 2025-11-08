using Ardalis.Result;
using HelloDoctorApi.Application.Payments.Models;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Common.Interfaces;

/// <summary>
/// Abstraction for payment gateway providers (PayFast, Stripe, PayPal, etc.)
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// The provider this gateway implementation is for
    /// </summary>
    PaymentProvider Provider { get; }

    /// <summary>
    /// Initiates a payment and returns payment URL for user to complete payment
    /// </summary>
    Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(
        PaymentInitiationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies payment status from payment provider
    /// </summary>
    Task<Result<PaymentVerificationResponse>> VerifyPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes callback/webhook from payment provider
    /// </summary>
    Task<Result<PaymentCallbackResponse>> ProcessCallbackAsync(
        Dictionary<string, string> callbackData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refunds a completed payment
    /// </summary>
    Task<Result<PaymentRefundResponse>> RefundPaymentAsync(
        string externalTransactionId,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates webhook signature to ensure authenticity
    /// </summary>
    bool ValidateWebhookSignature(Dictionary<string, string> data, string signature);
}
