namespace HelloDoctorApi.Application.Authentications.Models
{
    public class CreateUserRequest
    {
        public required string Email { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
    }
}
