using Microsoft.AspNetCore.Identity;

namespace HelloDoctorApi.Domain.Entities.Auth;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
    public bool IsActive { get; set; }
    public SystemAdministrator? SystemAdministrator { get; set; }
    public SuperAdministrator? SuperAdministrator { get; set; }
    public MainMember? MainMember { get; set; }
    public Pharmacist? Pharmacist { get; set; }
    public Doctor? Doctor { get; set; }
}