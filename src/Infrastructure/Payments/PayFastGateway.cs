using System.Security.Cryptography;
using System.Text;
using System.Web;
using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Payments.Models;
using HelloDoctorApi.Domain.Enums;
using Microsoft.Extensions.Options;

namespace HelloDoctorApi.Infrastructure.Payments;

/// <summary>
/// PayFast payment gateway implementation
/// Documentation: https://developers.payfast.co.za/documentation/
/// </summary>
public class PayFastGateway : IPaymentGateway
{
    private readonly PayFastSettings _settings;
    private readonly IApplicationDbContext _context;

    public PayFastGateway(IOptions<PayFastSettings> settings, IApplicationDbContext context)
    {
        _settings = settings.Value;
        _context = context;
    }

    public PaymentProvider Provider => PaymentProvider.PayFast;

    public async Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(
        PaymentInitiationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Build PayFast payment data
            var paymentData = new Dictionary<string, string>
            {
                { "merchant_id", _settings.MerchantId },
                { "merchant_key", _settings.MerchantKey },
                { "return_url", request.ReturnUrl },
                { "cancel_url", request.CancelUrl },
                { "notify_url", request.NotifyUrl },
                { "name_first", request.PayerName.Split(' ').FirstOrDefault() ?? request.PayerName },
                { "name_last", request.PayerName.Split(' ').Skip(1).FirstOrDefault() ?? "" },
                { "email_address", request.PayerEmail },
                { "m_payment_id", request.PaymentId.ToString() },
                { "amount", request.Amount.ToString("F2") },
                { "item_name", GetItemName(request.Purpose) },
                { "item_description", GetItemDescription(request.Purpose, request.PrescriptionId) },
                { "custom_str1", request.Purpose.ToString() },
                { "custom_int1", request.PrescriptionId?.ToString() ?? "0" }
            };

            // Generate signature
            var signature = GenerateSignature(paymentData);
            paymentData.Add("signature", signature);

            // Build payment URL
            var paymentUrl = $"{_settings.GetPaymentUrl()}?{BuildQueryString(paymentData)}";

            return Result.Success(new PaymentInitiationResponse(
                Success: true,
                PaymentUrl: paymentUrl,
                ExternalReference: request.PaymentId.ToString()
            ));
        }
        catch (Exception ex)
        {
            return Result<PaymentInitiationResponse>.Error(new[] { ex.Message });
        }
    }

    public async Task<Result<PaymentVerificationResponse>> VerifyPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // PayFast doesn't have a direct verification API
            // Verification is done through ITN (Instant Transaction Notification) callbacks
            // Check payment status in database
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == long.Parse(paymentId), cancellationToken);

            if (payment == null)
            {
                return Result<PaymentVerificationResponse>.NotFound();
            }

            return Result.Success(new PaymentVerificationResponse(
                Success: true,
                Status: payment.Status,
                ExternalTransactionId: payment.ExternalTransactionId,
                AmountPaid: payment.Amount
            ));
        }
        catch (Exception ex)
        {
            return Result<PaymentVerificationResponse>.Error(new[] { ex.Message });
        }
    }

    public async Task<Result<PaymentCallbackResponse>> ProcessCallbackAsync(
        Dictionary<string, string> callbackData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validate signature
            if (!ValidateWebhookSignature(callbackData, callbackData.GetValueOrDefault("signature") ?? ""))
            {
                return Result<PaymentCallbackResponse>.Error("Invalid signature");
            }

            // 2. Extract payment data
            var paymentId = long.Parse(callbackData.GetValueOrDefault("m_payment_id") ?? "0");
            var paymentStatus = callbackData.GetValueOrDefault("payment_status");
            var amountGross = decimal.Parse(callbackData.GetValueOrDefault("amount_gross") ?? "0");
            var pfPaymentId = callbackData.GetValueOrDefault("pf_payment_id");

            // 3. Determine payment status
            var status = paymentStatus?.ToLower() switch
            {
                "complete" => PaymentStatus.Completed,
                "failed" => PaymentStatus.Failed,
                "cancelled" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Pending
            };

            return Result.Success(new PaymentCallbackResponse(
                Success: true,
                PaymentId: paymentId,
                Status: status,
                ExternalTransactionId: pfPaymentId,
                AmountPaid: amountGross
            ));
        }
        catch (Exception ex)
        {
            return Result<PaymentCallbackResponse>.Error(new[] { ex.Message });
        }
    }

    public async Task<Result<PaymentRefundResponse>> RefundPaymentAsync(
        string externalTransactionId,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default)
    {
        // PayFast refunds are done manually through their dashboard
        // or via API (requires additional setup)
        // For now, we'll mark it as manual refund required
        return Result<PaymentRefundResponse>.Error("PayFast refunds must be processed manually through the PayFast dashboard");
    }

    public bool ValidateWebhookSignature(Dictionary<string, string> data, string signature)
    {
        // Create a copy without signature field
        var dataToValidate = data
            .Where(kvp => kvp.Key != "signature")
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var generatedSignature = GenerateSignature(dataToValidate);
        return generatedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
    }

    #region Private Helper Methods

    private string GenerateSignature(Dictionary<string, string> data)
    {
        // Sort data alphabetically and create parameter string
        var sortedData = data.OrderBy(kvp => kvp.Key);
        var parameterString = string.Join("&", sortedData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));

        // Add passphrase if configured
        if (!string.IsNullOrEmpty(_settings.Passphrase))
        {
            parameterString += $"&passphrase={HttpUtility.UrlEncode(_settings.Passphrase)}";
        }

        // Generate MD5 hash
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(parameterString));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    private string BuildQueryString(Dictionary<string, string> data)
    {
        return string.Join("&", data.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
    }

    private string GetItemName(PaymentPurpose purpose)
    {
        return purpose switch
        {
            PaymentPurpose.PrescriptionFee => "Prescription Consultation Fee",
            PaymentPurpose.DispenseFee => "Medicine Dispensing Fee",
            PaymentPurpose.DeliveryFee => "Prescription Delivery Fee",
            PaymentPurpose.Refund => "Refund",
            _ => "Healthcare Service"
        };
    }

    private string GetItemDescription(PaymentPurpose purpose, long? prescriptionId)
    {
        var description = purpose switch
        {
            PaymentPurpose.PrescriptionFee => "Payment for doctor consultation and prescription",
            PaymentPurpose.DispenseFee => "Payment for medicine dispensing at pharmacy",
            PaymentPurpose.DeliveryFee => "Payment for prescription delivery service",
            PaymentPurpose.Refund => "Refund for cancelled prescription",
            _ => "Payment for healthcare services"
        };

        if (prescriptionId.HasValue)
        {
            description += $" (Prescription #{prescriptionId})";
        }

        return description;
    }

    #endregion
}
