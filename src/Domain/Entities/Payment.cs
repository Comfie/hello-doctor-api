using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public required long PrescriptionId { get; set; }
    public required Prescription Prescription { get; set; }

    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }

    public required decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    public required PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public required PaymentMethod Method { get; set; }

    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public string? PaymentGatewayResponse { get; set; }

    public DateTimeOffset? PaymentDate { get; set; }
    public string? Notes { get; set; }

    // Navigation to invoice
    public long? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
}
