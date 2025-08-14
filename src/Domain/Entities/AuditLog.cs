using HelloDoctorApi.Domain.Common;

namespace HelloDoctorApi.Domain.Entities;

public class AuditLog : BaseAuditableEntity
{
    public required string Action { get; set; }
    public string? Details { get; set; }
    public string? ActorUserId { get; set; }
    public long? PrescriptionId { get; set; }
    public long? PharmacyId { get; set; }
}

