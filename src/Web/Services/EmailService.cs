using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Common.Models;
using ApiBaseTemplate.Application.Common.Models.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ApiBaseTemplate.Web.Services;

public class EmailService : IEmailService
{
    private ILogger<EmailService> _logger;
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(Message message,
        CancellationToken cancellationToken = default)
    {
        var emailMessage = CreateEmailMessage(message);
        return await SendAsync(emailMessage);
    }

    public void SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);

        Send(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("email", "_emailConfig.From"));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

        if (message.Attachments != null && message.Attachments.Any())
        {
            byte[] fileBytes;
            foreach (var attachment in message.Attachments)
            {
                using (var ms = new MemoryStream())
                {
                    attachment.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
            }
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private async Task<bool> SendAsync(MimeMessage mailMessage)
    {
        var host = _emailSettings.Host;
        var port = _emailSettings.Port;
        var password = _emailSettings.Password;
        var userName = _emailSettings.Username;

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(host, port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(userName, password);
            await client.SendAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return false;
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }

        return true;
    }

    private void Send(MimeMessage mailMessage)
    {
        var host = _emailSettings.Host;
        var port = _emailSettings.Port;
        var password = _emailSettings.Password;
        var userName = _emailSettings.Username;

        using var client = new SmtpClient();
        try
        {
            client.ConnectAsync(host, port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.AuthenticateAsync(userName, password);
            client.SendAsync(mailMessage);

            client.Send(mailMessage);
        }
        catch
        {
            //log an error message or throw an exception, or both.
            throw;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
    }

}
