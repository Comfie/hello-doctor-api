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

        // If payment is for a prescription, you can update prescription status here
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

                // TODO: Based on business rules, you may want to:
                // - Update prescription status (e.g., from Pending to Approved after payment)
                // - Send notification to doctor/pharmacy
                // - Create invoice record
                // Example:
                // if (prescription.Status == PrescriptionStatus.Pending)
                // {
                //     prescription.Status = PrescriptionStatus.Approved;
                //     await _context.SaveChangesAsync(cancellationToken);
                // }
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
