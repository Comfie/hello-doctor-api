using HelloDoctorApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace HelloDoctorApi.Application.Payments.EventHandlers;

/// <summary>
/// Handles PaymentInitiatedEvent for logging and tracking
/// </summary>
public class PaymentInitiatedEventHandler : INotificationHandler<PaymentInitiatedEvent>
{
    private readonly ILogger<PaymentInitiatedEventHandler> _logger;

    public PaymentInitiatedEventHandler(ILogger<PaymentInitiatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PaymentInitiatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Payment {PaymentId} initiated by {PayerId}. Amount: {Amount}, Purpose: {Purpose}, Provider: {Provider}",
            notification.PaymentId,
            notification.PayerId,
            notification.Amount,
            notification.Purpose,
            notification.Provider
        );

        // TODO: Additional actions after payment initiation
        // - Send payment initiation notification to payer
        // - Track payment initiation metrics
        // - Start payment timeout timer
        // - Log to analytics

        return Task.CompletedTask;
    }
}
