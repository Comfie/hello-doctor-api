using Ardalis.Result;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Application.Payments.Commands.ProcessPaymentCallback;

public record ProcessPaymentCallbackCommand(
    PaymentProvider Provider,
    Dictionary<string, string> CallbackData
) : IRequest<Result<bool>>;
