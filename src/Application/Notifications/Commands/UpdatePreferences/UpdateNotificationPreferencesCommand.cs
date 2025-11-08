using Ardalis.Result;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Notifications.Commands.UpdatePreferences;

public record NotificationPreferenceUpdate(
    NotificationType NotificationType,
    bool IsEnabled,
    NotificationChannel PreferredChannel
);

public record UpdateNotificationPreferencesCommand(
    List<NotificationPreferenceUpdate> Preferences
) : IRequest<Result>;
