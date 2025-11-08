using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public required string UserId { get; set; }
    public required ApplicationUser User { get; set; }

    public required NotificationType Type { get; set; }
    public required NotificationChannel Channel { get; set; }

    public required string Subject { get; set; }
    public required string Message { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset SentAt { get; set; }

    // Optional reference to related entities
    public long? PrescriptionId { get; set; }
    public Prescription? Prescription { get; set; }

    public long? PaymentId { get; set; }
}
