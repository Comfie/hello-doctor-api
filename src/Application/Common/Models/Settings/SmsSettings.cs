namespace ApiBaseTemplate.Application.Common.Models.Settings;

public class SmsSettings
{
    public const string Section = "SmsSettings";

    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? BaseAddress { get; set; }
    public string? From { get; set; }
    public string? Action { get; set; }
}
