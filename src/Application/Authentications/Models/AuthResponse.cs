namespace ApiBaseTemplate.Application.Authentications.Models
{
    public record AuthResponse
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public string? JwtToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public string? Message { get; set; }
    }
}
