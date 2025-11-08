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
                p.Id,
                p.Status.ToString(),
                p.Amount,
                p.Currency,
                p.Purpose.ToString(),
                p.ExternalTransactionId,
                p.CompletedAt,
                p.FailureReason
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            return Result<PaymentStatusResponse>.NotFound("Payment not found");
        }

        return Result.Success(payment);
    }
}
