namespace HelloDoctorApi.Infrastructure.Payments;

/// <summary>
/// PayFast payment gateway configuration settings
/// </summary>
public class PayFastSettings
{
    public const string SectionName = "PayFast";

    public string MerchantId { get; set; } = string.Empty;
    public string MerchantKey { get; set; } = string.Empty;
    public string Passphrase { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = "https://www.payfast.co.za/eng/process";
    public string SandboxPaymentUrl { get; set; } = "https://sandbox.payfast.co.za/eng/process";
    public bool UseSandbox { get; set; } = true;

    public string GetPaymentUrl() => UseSandbox ? SandboxPaymentUrl : PaymentUrl;
}
