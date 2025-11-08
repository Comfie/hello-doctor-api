using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Payments.Models;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HelloDoctorApi.Application.Payments.Commands.InitiatePayment;

public class InitiatePaymentHandler : IRequestHandler<InitiatePaymentCommand, Result<InitiatePaymentResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IEnumerable<IPaymentGateway> _paymentGateways;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InitiatePaymentHandler(
        IApplicationDbContext context,
        IUser user,
        IEnumerable<IPaymentGateway> paymentGateways,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _user = user;
        _paymentGateways = paymentGateways;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<InitiatePaymentResponse>> Handle(
        InitiatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        // Validate user is authenticated
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            return Result<InitiatePaymentResponse>.Forbidden();
        }

        // Get user details
        var payer = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == _user.Id, cancellationToken);

        if (payer == null)
        {
            return Result<InitiatePaymentResponse>.NotFound("User not found");
        }

        // Create payment record in database
        var payment = new Payment
        {
            PayerId = _user.Id!,
            Payer = payer,
            PayeeId = request.PayeeId,
            PayeeType = request.PayeeType,
            Amount = request.Amount,
            Currency = "ZAR", // Default for PayFast
            Purpose = request.Purpose,
            Status = PaymentStatus.Pending,
            Method = PaymentMethod.CreditCard, // Will be determined by gateway
            Provider = request.Provider,
            PrescriptionId = request.PrescriptionId,
            InitiatedAt = DateTimeOffset.UtcNow,
            Notes = request.Notes
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        // Get the appropriate payment gateway
        var gateway = _paymentGateways.FirstOrDefault(g => g.Provider == request.Provider);
        if (gateway == null)
        {
            payment.MarkAsFailed($"Payment gateway {request.Provider} not configured");
            await _context.SaveChangesAsync(cancellationToken);
            return Result<InitiatePaymentResponse>.Error($"Payment provider {request.Provider} is not available");
        }

        // Build callback URLs
        var baseUrl = GetBaseUrl();
        var returnUrl = $"{baseUrl}/payment/return/{payment.Id}";
        var cancelUrl = $"{baseUrl}/payment/cancel/{payment.Id}";
        var notifyUrl = $"{baseUrl}/api/v1/payment/callback/{request.Provider.ToString().ToLower()}";

        // Initiate payment with gateway
        var initiationRequest = new PaymentInitiationRequest(
            PaymentId: payment.Id,
            Amount: payment.Amount,
            Currency: payment.Currency,
            Purpose: payment.Purpose,
            PrescriptionId: payment.PrescriptionId,
            PayerEmail: payer.Email ?? "",
            PayerName: $"{payer.FirstName} {payer.LastName}",
            ReturnUrl: returnUrl,
            CancelUrl: cancelUrl,
            NotifyUrl: notifyUrl
        );

        var initiationResult = await gateway.InitiatePaymentAsync(initiationRequest, cancellationToken);

        if (!initiationResult.IsSuccess || initiationResult.Value.PaymentUrl == null)
        {
            payment.MarkAsFailed(initiationResult.Value.ErrorMessage ?? "Failed to initiate payment");
            await _context.SaveChangesAsync(cancellationToken);
            return Result<InitiatePaymentResponse>.Error(initiationResult.Errors.ToArray());
        }

        // Update payment with payment URL
        payment.PaymentUrl = initiationResult.Value.PaymentUrl;
        payment.ExternalTransactionId = initiationResult.Value.ExternalReference;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new InitiatePaymentResponse(
            PaymentId: payment.Id,
            PaymentUrl: payment.PaymentUrl,
            Status: payment.Status.ToString()
        ));
    }

    private string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            return $"{request.Scheme}://{request.Host}";
        }

        // Fallback to configuration
        return _configuration["AppSettings:BaseUrl"] ?? "http://localhost:5000";
    }
}
