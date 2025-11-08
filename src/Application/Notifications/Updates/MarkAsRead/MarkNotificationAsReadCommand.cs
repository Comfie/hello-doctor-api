using Ardalis.Result;

namespace HelloDoctorApi.Application.Notifications.Updates.MarkAsRead;

public record MarkNotificationAsReadCommand(long NotificationId) : IRequest<Result>;
