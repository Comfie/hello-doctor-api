using System.Net;
using System.Net.Mail;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Common.Models.Settings;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HelloDoctorApi.Infrastructure.Services.Notifications;

public class EmailNotificationService : INotificationService
{
    private readonly EmailSettings _emailSettings;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        IOptions<EmailSettings> emailSettings,
        IApplicationDbContext context,
        ILogger<EmailNotificationService> logger)
    {
        _emailSettings = emailSettings.Value;
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendNotificationAsync(
        string userId,
        NotificationType notificationType,
        string subject,
        string message,
        NotificationChannel channel = NotificationChannel.Email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get user details
            var user = await _context.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for notification", userId);
                return false;
            }

            // Check user's notification preferences
            var preference = await _context.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(np => np.UserId == userId && np.NotificationType == notificationType, cancellationToken);

            // If user has explicitly disabled this notification type, don't send
            if (preference != null && !preference.IsEnabled)
            {
                _logger.LogInformation("Notification {Type} disabled for user {UserId}", notificationType, userId);
                return false;
            }

            // Determine which channel to use (default to email if no preference)
            var channelToUse = preference?.PreferredChannel ?? channel;

            bool sent = false;
            switch (channelToUse)
            {
                case NotificationChannel.Email:
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        sent = await SendEmailAsync(user.Email, subject, message, cancellationToken);
                    }
                    break;

                case NotificationChannel.Sms:
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        sent = await SendSmsAsync(user.PhoneNumber, message, cancellationToken);
                    }
                    break;

                case NotificationChannel.InApp:
                    // Store in-app notification
                    sent = await StoreInAppNotificationAsync(userId, notificationType, subject, message, cancellationToken);
                    break;
            }

            return sent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            var fromEmail = string.IsNullOrEmpty(_emailSettings.FromEmail)
                ? _emailSettings.Username
                : _emailSettings.FromEmail;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement SMS sending using Twilio, AWS SNS, or local provider
        _logger.LogWarning("SMS sending not yet implemented for {PhoneNumber}", phoneNumber);
        await Task.CompletedTask;
        return false;
    }

    private async Task<bool> StoreInAppNotificationAsync(
        string userId,
        NotificationType notificationType,
        string subject,
        string message,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for in-app notification", userId);
                return false;
            }

            var notification = new Domain.Entities.Notification
            {
                UserId = userId,
                User = user,
                Type = notificationType,
                Channel = NotificationChannel.InApp,
                Subject = subject,
                Message = message,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("In-app notification stored for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store in-app notification for user {UserId}", userId);
            return false;
        }
    }
}
