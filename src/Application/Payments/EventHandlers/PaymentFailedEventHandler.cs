using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace HelloDoctorApi.Application.Payments.EventHandlers;

/// <summary>
/// Handles PaymentFailedEvent to log failures and notify users
/// </summary>
public class PaymentFailedEventHandler : INotificationHandler<PaymentFailedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PaymentFailedEventHandler> _logger;

    public PaymentFailedEventHandler(
        IApplicationDbContext context,
        ILogger<PaymentFailedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Payment {PaymentId} failed for {Purpose}. Amount: {Amount}, Payer: {PayerId}, Reason: {Reason}",
            notification.PaymentId,
            notification.Purpose,
            notification.Amount,
            notification.PayerId,
            notification.FailureReason
        );

        // If payment is for a prescription, you may want to take specific action
        if (notification.PrescriptionId.HasValue)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == notification.PrescriptionId.Value, cancellationToken);

            if (prescription != null)
            {
                _logger.LogWarning(
                    "Payment failure for Prescription {PrescriptionId}. Prescription may need attention.",
                    prescription.Id
                );

                // TODO: Based on business rules, you may want to:
                // - Mark prescription as payment failed
                // - Send notification to user to retry payment
                // - Add note to prescription about payment failure
                // - Set a retry counter or expiry time
            }
        }

        // TODO: Additional actions after payment failure
        // - Send payment failure notification to payer
        // - Log to external monitoring system
        // - Track failed payment metrics
        // - Possibly trigger retry mechanism with exponential backoff

        await Task.CompletedTask;
    }
}
