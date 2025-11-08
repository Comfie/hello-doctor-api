using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Events;

/// <summary>
/// Raised when a payment is initiated
/// </summary>
public sealed class PaymentInitiatedEvent : BaseEvent
{
    public long PaymentId { get; }
    public string PayerId { get; }
    public decimal Amount { get; }
    public PaymentPurpose Purpose { get; }
    public long? PrescriptionId { get; }
    public PaymentProvider Provider { get; }

    public PaymentInitiatedEvent(
        long paymentId,
        string payerId,
        decimal amount,
        PaymentPurpose purpose,
        long? prescriptionId,
        PaymentProvider provider)
    {
        PaymentId = paymentId;
        PayerId = payerId;
        Amount = amount;
        Purpose = purpose;
        PrescriptionId = prescriptionId;
        Provider = provider;
    }
}

/// <summary>
/// Raised when a payment is successfully completed
/// </summary>
public sealed class PaymentCompletedEvent : BaseEvent
{
    public long PaymentId { get; }
    public string PayerId { get; }
    public decimal Amount { get; }
    public PaymentPurpose Purpose { get; }
    public long? PrescriptionId { get; }
    public string? PayeeId { get; }
    public string? PayeeType { get; }
    public string ExternalTransactionId { get; }
    public PaymentProvider Provider { get; }
    public DateTimeOffset CompletedAt { get; }

    public PaymentCompletedEvent(
        long paymentId,
        string payerId,
        decimal amount,
        PaymentPurpose purpose,
        long? prescriptionId,
        string? payeeId,
        string? payeeType,
        string externalTransactionId,
        PaymentProvider provider,
        DateTimeOffset completedAt)
    {
        PaymentId = paymentId;
        PayerId = payerId;
        Amount = amount;
        Purpose = purpose;
        PrescriptionId = prescriptionId;
        PayeeId = payeeId;
        PayeeType = payeeType;
        ExternalTransactionId = externalTransactionId;
        Provider = provider;
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Raised when a payment fails
/// </summary>
public sealed class PaymentFailedEvent : BaseEvent
{
    public long PaymentId { get; }
    public string PayerId { get; }
    public decimal Amount { get; }
    public PaymentPurpose Purpose { get; }
    public long? PrescriptionId { get; }
    public string FailureReason { get; }
    public PaymentProvider Provider { get; }
    public DateTimeOffset FailedAt { get; }

    public PaymentFailedEvent(
        long paymentId,
        string payerId,
        decimal amount,
        PaymentPurpose purpose,
        long? prescriptionId,
        string failureReason,
        PaymentProvider provider,
        DateTimeOffset failedAt)
    {
        PaymentId = paymentId;
        PayerId = payerId;
        Amount = amount;
        Purpose = purpose;
        PrescriptionId = prescriptionId;
        FailureReason = failureReason;
        Provider = provider;
        FailedAt = failedAt;
    }
}

/// <summary>
/// Raised when a payment is refunded
/// </summary>
public sealed class PaymentRefundedEvent : BaseEvent
{
    public long PaymentId { get; }
    public string PayerId { get; }
    public decimal Amount { get; }
    public PaymentPurpose Purpose { get; }
    public long? PrescriptionId { get; }
    public string RefundReason { get; }
    public PaymentProvider Provider { get; }
    public DateTimeOffset RefundedAt { get; }

    public PaymentRefundedEvent(
        long paymentId,
        string payerId,
        decimal amount,
        PaymentPurpose purpose,
        long? prescriptionId,
        string refundReason,
        PaymentProvider provider,
        DateTimeOffset refundedAt)
    {
        PaymentId = paymentId;
        PayerId = payerId;
        Amount = amount;
        Purpose = purpose;
        PrescriptionId = prescriptionId;
        RefundReason = refundReason;
        Provider = provider;
        RefundedAt = refundedAt;
    }
}

/// <summary>
/// Raised when a payment is cancelled
/// </summary>
public sealed class PaymentCancelledEvent : BaseEvent
{
    public long PaymentId { get; }
    public string PayerId { get; }
    public decimal Amount { get; }
    public PaymentPurpose Purpose { get; }
    public long? PrescriptionId { get; }
    public PaymentProvider Provider { get; }

    public PaymentCancelledEvent(
        long paymentId,
        string payerId,
        decimal amount,
        PaymentPurpose purpose,
        long? prescriptionId,
        PaymentProvider provider)
    {
        PaymentId = paymentId;
        PayerId = payerId;
        Amount = amount;
        Purpose = purpose;
        PrescriptionId = prescriptionId;
        Provider = provider;
    }
}
