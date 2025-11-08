using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Notifications.Models;

public record NotificationResponse(
    long Id,
    NotificationType Type,
    NotificationChannel Channel,
    string Subject,
    string Message,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset SentAt,
    long? PrescriptionId
);
