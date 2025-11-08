using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Entities;

public class NotificationPreference : BaseAuditableEntity
{
    public required string UserId { get; set; }
    public required ApplicationUser User { get; set; }

    public required NotificationType NotificationType { get; set; }
    public bool IsEnabled { get; set; } = true;
    public NotificationChannel PreferredChannel { get; set; } = NotificationChannel.Email;
}
