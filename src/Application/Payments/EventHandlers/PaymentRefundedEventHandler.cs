using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace HelloDoctorApi.Application.Payments.EventHandlers;

/// <summary>
/// Handles PaymentRefundedEvent to update related entities and notify users
/// </summary>
public class PaymentRefundedEventHandler : INotificationHandler<PaymentRefundedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PaymentRefundedEventHandler> _logger;

    public PaymentRefundedEventHandler(
        IApplicationDbContext context,
        ILogger<PaymentRefundedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(PaymentRefundedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Payment {PaymentId} refunded. Amount: {Amount}, Reason: {Reason}, Payer: {PayerId}",
            notification.PaymentId,
            notification.Amount,
            notification.RefundReason,
            notification.PayerId
        );

        // If payment was for a prescription, update prescription status
        if (notification.PrescriptionId.HasValue)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == notification.PrescriptionId.Value, cancellationToken);

            if (prescription != null)
            {
                _logger.LogInformation(
                    "Payment refund for Prescription {PrescriptionId}. May need to revert prescription status.",
                    prescription.Id
                );

                // TODO: Based on business rules, you may want to:
                // - Revert prescription status to previous state
                // - Add note about refund to prescription
                // - Notify doctor/pharmacy about refund
            }
        }

        // TODO: Additional actions after payment refund
        // - Send refund confirmation to payer
        // - Notify payee about refund
        // - Update invoice/accounting records
        // - Track refund metrics

        await Task.CompletedTask;
    }
}
