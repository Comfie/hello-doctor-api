using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Domain.Entities;

public class PrescriptionStatusHistory
{
    public long Id { get; set; }
    public long PrescriptionId { get; set; }
    public PrescriptionStatus OldStatus { get; set; }
    public PrescriptionStatus NewStatus { get; set; }
    public required string ChangedByUserId { get; set; }
    public DateTimeOffset ChangedDate { get; set; }
    public required string Reason { get; set; }
    public required Prescription Prescription { get; set; }
    public required ApplicationUser ChangedByUser { get; set; }
}