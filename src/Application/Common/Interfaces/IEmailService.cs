using ApiBaseTemplate.Application.Common.Models;

namespace ApiBaseTemplate.Application.Common.Interfaces;

public interface IEmailService 
{
    Task<bool> SendEmailAsync(Message message,
        CancellationToken cancellationToken = default);
    void SendEmail(Message message);
}
