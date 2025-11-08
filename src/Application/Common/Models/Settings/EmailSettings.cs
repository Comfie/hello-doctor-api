namespace HelloDoctorApi.Application.Common.Models.Settings
{
    public class EmailSettings
    {
        public const string Section = "EmailSettings";

        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Hello Doctor";
        public bool EnableSsl { get; set; } = true;
    }
}