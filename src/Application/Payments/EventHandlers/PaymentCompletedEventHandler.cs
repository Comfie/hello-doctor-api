using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace HelloDoctorApi.Application.Payments.EventHandlers;

/// <summary>
/// Handles PaymentCompletedEvent to update prescription status or trigger notifications
/// </summary>
public class PaymentCompletedEventHandler : INotificationHandler<PaymentCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PaymentCompletedEventHandler> _logger;

    public PaymentCompletedEventHandler(
        IApplicationDbContext context,
        ILogger<PaymentCompletedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Payment {PaymentId} completed for {Purpose}. Amount: {Amount}, Payer: {PayerId}, ExternalTxId: {ExternalTxId}",
            notification.PaymentId,
            notification.Purpose,
            notification.Amount,
            notification.PayerId,
            notification.ExternalTransactionId
        );

        // If payment is for a prescription, update prescription status
        if (notification.PrescriptionId.HasValue)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == notification.PrescriptionId.Value, cancellationToken);

            if (prescription != null)
            {
                _logger.LogInformation(
                    "Payment {PaymentId} is linked to Prescription {PrescriptionId}. Current status: {Status}",
                    notification.PaymentId,
                    prescription.Id,
                    prescription.Status
                );

                // Approve prescription after successful payment
                prescription.ApproveAfterPayment();

                // Add a note about the payment
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
                        Note = $"Payment completed successfully. Transaction ID: {notification.ExternalTransactionId}. Amount: R{notification.Amount:F2}",
                        CreatedDate = DateTimeOffset.UtcNow,
                        IsPrivate = false,
                        IsSystemGenerated = true
                    };
                    _context.PrescriptionNotes.Add(note);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Prescription {PrescriptionId} approved after successful payment. New status: {Status}",
                    prescription.Id,
                    prescription.Status
                );
            }
            else
            {
                _logger.LogWarning(
                    "Payment {PaymentId} references non-existent Prescription {PrescriptionId}",
                    notification.PaymentId,
                    notification.PrescriptionId.Value
                );
            }
        }

        // TODO: Additional actions after payment completion
        // - Send payment confirmation email/SMS to payer
        // - Send notification to payee (Doctor/Pharmacy)
        // - Update invoice status if applicable
        // - Trigger analytics/reporting events

        await Task.CompletedTask;
    }
}
