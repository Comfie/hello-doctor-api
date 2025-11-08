namespace HelloDoctorApi.Domain.Entities;

public class InvoiceLineItem : BaseAuditableEntity
{
    public required long InvoiceId { get; set; }
    public required Invoice Invoice { get; set; }

    public required string Description { get; set; }
    public int Quantity { get; set; } = 1;
    public required decimal UnitPrice { get; set; }
    public required decimal TotalPrice { get; set; }

    public string? MedicationName { get; set; }
    public string? Notes { get; set; }
}
