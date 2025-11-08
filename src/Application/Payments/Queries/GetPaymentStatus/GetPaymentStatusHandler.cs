using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Payments.Queries.GetPaymentStatus;

public class GetPaymentStatusHandler : IRequestHandler<GetPaymentStatusQuery, Result<PaymentStatusResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetPaymentStatusHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<PaymentStatusResponse>> Handle(
        GetPaymentStatusQuery request,
        CancellationToken cancellationToken)
    {
        // Validate user is authenticated
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            return Result<PaymentStatusResponse>.Forbidden();
        }

        var payment = await _context.Payments
            .Where(p => p.Id == request.PaymentId)
            .Where(p => p.PayerId == _user.Id) // Only payer can view their payment
            .Select(p => new PaymentStatusResponse(
                PaymentId: p.Id,
                Status: p.Status.ToString(),
                Amount: p.Amount,
                Currency: p.Currency,
                Purpose: p.Purpose.ToString(),
                ExternalTransactionId: p.ExternalTransactionId,
                CompletedAt: p.CompletedAt,
                FailureReason: p.FailureReason
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            return Result<PaymentStatusResponse>.NotFound("Payment not found");
        }

        return Result.Success(payment);
    }
}
