namespace ApiBaseTemplate.Domain.Entities.Auth;

public class SuperAdministrator : BaseAuditableEntity
{
    public required string UserId { get; set; }
    public required ApplicationUser User { get; set; }
}