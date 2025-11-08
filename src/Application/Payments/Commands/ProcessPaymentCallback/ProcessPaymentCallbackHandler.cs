using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Enums;
using System.Text.Json;

namespace HelloDoctorApi.Application.Payments.Commands.ProcessPaymentCallback;

public class ProcessPaymentCallbackHandler : IRequestHandler<ProcessPaymentCallbackCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEnumerable<IPaymentGateway> _paymentGateways;

    public ProcessPaymentCallbackHandler(
        IApplicationDbContext context,
        IEnumerable<IPaymentGateway> paymentGateways)
    {
        _context = context;
        _paymentGateways = paymentGateways;
    }

    public async Task<Result<bool>> Handle(
        ProcessPaymentCallbackCommand request,
        CancellationToken cancellationToken)
    {
        // Get the appropriate payment gateway
        var gateway = _paymentGateways.FirstOrDefault(g => g.Provider == request.Provider);
        if (gateway == null)
        {
            return Result<bool>.Error($"Payment gateway {request.Provider} not found");
        }

        // Process callback through gateway
        var callbackResult = await gateway.ProcessCallbackAsync(request.CallbackData, cancellationToken);

        if (!callbackResult.IsSuccess)
        {
            return Result<bool>.Error(callbackResult.Errors);
        }

        var callbackResponse = callbackResult.Value;

        // Get payment from database
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == callbackResponse.PaymentId, cancellationToken);

        if (payment == null)
        {
            return Result<bool>.NotFound($"Payment {callbackResponse.PaymentId} not found");
        }

        // Serialize callback data for storage
        var callbackDataJson = JsonSerializer.Serialize(request.CallbackData);

        // Update payment based on status
        switch (callbackResponse.Status)
        {
            case PaymentStatus.Completed:
                payment.MarkAsCompleted(
                    callbackResponse.ExternalTransactionId ?? "UNKNOWN",
                    callbackDataJson
                );
                break;

            case PaymentStatus.Failed:
                payment.MarkAsFailed(
                    callbackResponse.ErrorMessage ?? "Payment failed",
                    callbackDataJson
                );
                break;

            case PaymentStatus.Cancelled:
                payment.MarkAsCancelled();
                payment.CallbackData = callbackDataJson;
                payment.CallbackReceivedAt = DateTimeOffset.UtcNow;
                break;

            default:
                // Update callback data but don't change status
                payment.CallbackData = callbackDataJson;
                payment.CallbackReceivedAt = DateTimeOffset.UtcNow;
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Trigger domain events for payment completion
        // E.g., PaymentCompletedEvent to update prescription status

        return Result.Success(true);
    }
}
