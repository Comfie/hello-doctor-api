using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryHandler : IRequestHandler<GetPaymentHistoryQuery, Result<List<PaymentHistoryItem>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetPaymentHistoryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<List<PaymentHistoryItem>>> Handle(
        GetPaymentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        // Validate user is authenticated
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            return Result<List<PaymentHistoryItem>>.Forbidden();
        }

        var skip = (request.Page - 1) * request.PageSize;

        var payments = await _context.Payments
            .Where(p => p.PayerId == _user.Id) // Only user's own payments
            .OrderByDescending(p => p.InitiatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(p => new PaymentHistoryItem(
                p.Id,
                p.Status.ToString(),
                p.Amount,
                p.Currency,
                p.Purpose.ToString(),
                p.Provider.ToString(),
                p.PrescriptionId,
                p.InitiatedAt!.Value,
                p.CompletedAt
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(payments);
    }
}
