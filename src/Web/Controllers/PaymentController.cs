using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Payments.Commands.InitiatePayment;
using HelloDoctorApi.Application.Payments.Commands.ProcessPaymentCallback;
using HelloDoctorApi.Application.Payments.Queries.GetPaymentHistory;
using HelloDoctorApi.Application.Payments.Queries.GetPaymentStatus;
using HelloDoctorApi.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PaymentController : ApiController
{
    public PaymentController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Initiate a payment for prescription or dispensing
    /// </summary>
    /// <param name="request">Payment initiation details</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Payment URL to redirect user for payment</returns>
    [HttpPost("initiate")]
    [Authorize(Roles = "MainMember")]
    [ProducesResponseType(typeof(InitiatePaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<Result<InitiatePaymentResponse>> InitiatePayment(
        [FromBody] InitiatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    /// Get payment status by payment ID
    /// </summary>
    /// <param name="paymentId">Payment identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Payment status details</returns>
    [HttpGet("{paymentId:long}/status")]
    [Authorize(Roles = "MainMember,Doctor,Pharmacist,SuperAdministrator")]
    [ProducesResponseType(typeof(PaymentStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result<PaymentStatusResponse>> GetPaymentStatus(
        long paymentId,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetPaymentStatusQuery(paymentId), cancellationToken);
        return response;
    }

    /// <summary>
    /// Get user's payment history with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of user's payments</returns>
    [HttpGet("history")]
    [Authorize(Roles = "MainMember")]
    [ProducesResponseType(typeof(List<PaymentHistoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result<List<PaymentHistoryItem>>> GetPaymentHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new GetPaymentHistoryQuery(page, pageSize), cancellationToken);
        return response;
    }

    /// <summary>
    /// Webhook endpoint for PayFast payment notifications
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>OK if processed successfully</returns>
    [HttpPost("callback/payfast")]
    [AllowAnonymous] // Webhooks come from external services
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PayFastCallback(CancellationToken cancellationToken)
    {
        try
        {
            // Read form data from request
            var formData = Request.Form.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToString()
            );

            var command = new ProcessPaymentCallbackCommand(
                PaymentProvider.PayFast,
                formData
            );

            var result = await Sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Webhook endpoint for Stripe payment notifications
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>OK if processed successfully</returns>
    [HttpPost("callback/stripe")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> StripeCallback(CancellationToken cancellationToken)
    {
        // TODO: Implement when Stripe gateway is added
        return Task.FromResult<IActionResult>(Ok());
    }

    /// <summary>
    /// Webhook endpoint for PayPal payment notifications
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>OK if processed successfully</returns>
    [HttpPost("callback/paypal")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> PayPalCallback(CancellationToken cancellationToken)
    {
        // TODO: Implement when PayPal gateway is added
        return Task.FromResult<IActionResult>(Ok());
    }
}
