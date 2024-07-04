namespace ApiBaseTemplate.Application.Common.Models.Settings;

public class AppSettings
{
    public const string Section = "AppSettings";

    public string Secret { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string WebUrl { get; set; } = string.Empty;
}
