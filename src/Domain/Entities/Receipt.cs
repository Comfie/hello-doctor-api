namespace HelloDoctorApi.Domain.Entities;

public class Receipt : BaseAuditableEntity
{
    public required string ReceiptNumber { get; set; }

    public required long PaymentId { get; set; }
    public required Payment Payment { get; set; }

    public required long InvoiceId { get; set; }
    public required Invoice Invoice { get; set; }

    public required DateTimeOffset IssuedDate { get; set; }

    public string? Notes { get; set; }
}
