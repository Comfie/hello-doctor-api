using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Notifications.Models;

public record NotificationPreferenceResponse(
    long Id,
    NotificationType NotificationType,
    bool IsEnabled,
    NotificationChannel PreferredChannel
);
