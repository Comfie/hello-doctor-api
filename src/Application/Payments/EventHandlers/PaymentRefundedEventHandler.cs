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

                // Revert prescription to PaymentPending status
                if (prescription.Status == Domain.Enums.PrescriptionStatus.Approved)
                {
                    prescription.Status = Domain.Enums.PrescriptionStatus.PaymentPending;
                }

                // Add a note about the refund
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
                        Note = $"Payment refunded. Reason: {notification.RefundReason}. Amount: R{notification.Amount:F2}. Prescription reverted to PaymentPending status.",
                        CreatedDate = DateTimeOffset.UtcNow,
                        IsPrivate = false,
                        IsSystemGenerated = true
                    };
                    _context.PrescriptionNotes.Add(note);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Prescription {PrescriptionId} reverted to PaymentPending after refund. Status: {Status}",
                    prescription.Id,
                    prescription.Status
                );
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
