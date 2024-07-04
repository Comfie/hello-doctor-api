namespace ApiBaseTemplate.Application.Authentications.Models
{
    public class UpdateUserPasswordRequest
    {
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
