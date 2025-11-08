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

        // If payment is for a prescription, update prescription status
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

                // Mark prescription as on hold due to failed payment
                prescription.RejectDueToFailedPayment(notification.FailureReason);

                // Add a note about the payment failure
                var payer = await _context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == notification.PayerId, cancellationToken);

                if (payer != null)
                {
                    var note = new Domain.Entities.PrescriptionNote
                    {
                        PrescriptionId = prescription.Id,
                        Prescription = prescription,
                        UserId = notification.PayerId,
                        User = payer,
                        UserType = Domain.Enums.UserType.MainMember,
                        Note = $"Payment failed. Reason: {notification.FailureReason}. Amount: R{notification.Amount:F2}. Please retry payment to continue.",
                        CreatedDate = DateTimeOffset.UtcNow,
                        IsPrivate = false,
                        IsSystemGenerated = true
                    };
                    _context.PrescriptionNotes.Add(note);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogWarning(
                    "Prescription {PrescriptionId} marked as OnHold due to payment failure. Status: {Status}",
                    prescription.Id,
                    prescription.Status
                );
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
