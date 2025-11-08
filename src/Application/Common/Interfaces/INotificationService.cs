using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Send a notification to a user
    /// </summary>
    Task<bool> SendNotificationAsync(
        string userId,
        NotificationType notificationType,
        string subject,
        string message,
        NotificationChannel channel = NotificationChannel.Email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send email notification
    /// </summary>
    Task<bool> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send SMS notification
    /// </summary>
    Task<bool> SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default);
}
