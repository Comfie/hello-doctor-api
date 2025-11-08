using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace HelloDoctorApi.Application.Payments.EventHandlers;

/// <summary>
/// Handles PaymentCancelledEvent to clean up and notify users
/// </summary>
public class PaymentCancelledEventHandler : INotificationHandler<PaymentCancelledEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PaymentCancelledEventHandler> _logger;

    public PaymentCancelledEventHandler(
        IApplicationDbContext context,
        ILogger<PaymentCancelledEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(PaymentCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Payment {PaymentId} cancelled by user. Amount: {Amount}, Purpose: {Purpose}, Payer: {PayerId}",
            notification.PaymentId,
            notification.Amount,
            notification.Purpose,
            notification.PayerId
        );

        // If payment was for a prescription, handle cancellation
        if (notification.PrescriptionId.HasValue)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == notification.PrescriptionId.Value, cancellationToken);

            if (prescription != null)
            {
                _logger.LogInformation(
                    "Payment cancellation for Prescription {PrescriptionId}. User may need to retry payment.",
                    prescription.Id
                );

                // TODO: Based on business rules, you may want to:
                // - Send reminder to user to complete payment
                // - Set prescription status to awaiting payment
                // - Add note about cancelled payment
                // - Track cancellation metrics
            }
        }

        // TODO: Additional actions after payment cancellation
        // - Send cancellation notification to payer
        // - Clean up temporary payment records
        // - Track cancellation reasons/patterns
        // - Possibly trigger re-engagement campaign

        await Task.CompletedTask;
    }
}
