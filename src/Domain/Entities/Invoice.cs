using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public required string InvoiceNumber { get; set; }

    public required long PrescriptionId { get; set; }
    public required Prescription Prescription { get; set; }

    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }

    public required long PharmacyId { get; set; }
    public required Pharmacy Pharmacy { get; set; }

    public required decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public required decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "USD";

    public required DateTimeOffset InvoiceDate { get; set; }
    public required DateTimeOffset DueDate { get; set; }

    public required InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<InvoiceLineItem>? LineItems { get; set; }
    public ICollection<Payment>? Payments { get; set; }
}
