using HelloDoctorApi.Application.Common.Models;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(Message message,
        CancellationToken cancellationToken = default);

    void SendEmail(Message message);
}