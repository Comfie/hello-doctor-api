using Microsoft.AspNetCore.Identity;

namespace HelloDoctorApi.Domain.Entities.Auth;

public class ApplicationRole : IdentityRole, ISoftDelete
{
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}