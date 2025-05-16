using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Domain.Entities;

public class Prescription : BaseAuditableEntity
{
    public string? Notes { get; set; }
    public DateTimeOffset IssuedDate { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public PrescriptionStatus Status { get; set; }
    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }
    public long BeneficiaryId { get; set; }
    public required Beneficiary Beneficiary { get; set; }
    public List<FileUpload>? PrescriptionFiles { get; set; } = new();
    public List<PrescriptionNote>? PrescriptionNotes { get; set; } = new();
    public List<PrescriptionStatusHistory>? StatusHistory { get; set; } = new();
}