namespace ApiBaseTemplate.Application.Common.Models.Settings
{
    public class EmailSettings
    {
        public const string Section = "EmailSettings";

        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

    }
}
