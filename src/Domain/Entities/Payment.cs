using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Events;

namespace HelloDoctorApi.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    // Payer Information
    public required string PayerId { get; set; }
    public required ApplicationUser Payer { get; set; }

    // Payee Information (Doctor or Pharmacy)
    public string? PayeeId { get; set; } // DoctorId or PharmacyId based on purpose
    public string? PayeeType { get; set; } // "Doctor" or "Pharmacy"

    // Payment Details
    public required decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR"; // South African Rand (default for PayFast)
    public required PaymentPurpose Purpose { get; set; }
    public required PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public required PaymentMethod Method { get; set; }
    public required PaymentProvider Provider { get; set; }

    // Reference to related entities
    public long? PrescriptionId { get; set; }
    public Prescription? Prescription { get; set; }

    // Payment Gateway Information
    public string? ExternalTransactionId { get; set; } // Transaction ID from payment provider
    public string? PaymentGatewayResponse { get; set; } // Raw response from gateway
    public string? PaymentUrl { get; set; } // URL to redirect user for payment

    // Callback/Webhook data
    public string? CallbackData { get; set; } // JSON data from payment callback
    public DateTimeOffset? CallbackReceivedAt { get; set; }

    // Payment Timeline
    public DateTimeOffset? InitiatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? FailedAt { get; set; }
    public DateTimeOffset? RefundedAt { get; set; }

    // Additional Information
    public string? Notes { get; set; }
    public string? FailureReason { get; set; }
    public string? RefundReason { get; set; }

    // Navigation to invoice
    public long? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    // Domain Methods
    public void MarkAsInitiated()
    {
        InitiatedAt = DateTimeOffset.UtcNow;
        Status = PaymentStatus.Pending;

        // Raise domain event
        AddDomainEvent(new PaymentInitiatedEvent(
            Id,
            PayerId,
            Amount,
            Purpose,
            PrescriptionId,
            Provider
        ));
    }

    public void MarkAsCompleted(string externalTransactionId, string? callbackData = null)
    {
        Status = PaymentStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        ExternalTransactionId = externalTransactionId;
        CallbackData = callbackData;
        CallbackReceivedAt = DateTimeOffset.UtcNow;

        // Raise domain event
        AddDomainEvent(new PaymentCompletedEvent(
            Id,
            PayerId,
            Amount,
            Purpose,
            PrescriptionId,
            PayeeId,
            PayeeType,
            externalTransactionId,
            Provider,
            CompletedAt.Value
        ));
    }

    public void MarkAsFailed(string reason, string? callbackData = null)
    {
        Status = PaymentStatus.Failed;
        FailedAt = DateTimeOffset.UtcNow;
        FailureReason = reason;
        CallbackData = callbackData;
        CallbackReceivedAt = DateTimeOffset.UtcNow;

        // Raise domain event
        AddDomainEvent(new PaymentFailedEvent(
            Id,
            PayerId,
            Amount,
            Purpose,
            PrescriptionId,
            reason,
            Provider,
            FailedAt.Value
        ));
    }

    public void MarkAsRefunded(string reason)
    {
        Status = PaymentStatus.Refunded;
        RefundedAt = DateTimeOffset.UtcNow;
        RefundReason = reason;

        // Raise domain event
        AddDomainEvent(new PaymentRefundedEvent(
            Id,
            PayerId,
            Amount,
            Purpose,
            PrescriptionId,
            reason,
            Provider,
            RefundedAt.Value
        ));
    }

    public void MarkAsCancelled()
    {
        Status = PaymentStatus.Cancelled;

        // Raise domain event
        AddDomainEvent(new PaymentCancelledEvent(
            Id,
            PayerId,
            Amount,
            Purpose,
            PrescriptionId,
            Provider
        ));
    }
}

