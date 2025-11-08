namespace HelloDoctorApi.Domain.Enums;

/// <summary>
/// Payment gateway providers supported by the system
/// </summary>
public enum PaymentProvider
{
    /// <summary>
    /// PayFast payment gateway (South Africa)
    /// </summary>
    PayFast = 0,

    /// <summary>
    /// Stripe payment gateway
    /// </summary>
    Stripe = 1,

    /// <summary>
    /// PayPal payment gateway
    /// </summary>
    PayPal = 2,

    /// <summary>
    /// Paystack payment gateway (Africa)
    /// </summary>
    Paystack = 3,

    /// <summary>
    /// Manual/offline payment
    /// </summary>
    Manual = 99
}
