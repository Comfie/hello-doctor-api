namespace ApiBaseTemplate.Domain.Entities.Auth;

public class OneTimePin : BaseAuditableEntity
{
    public int Counter { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public DateTime LastIssuedAt { get; set; }
}
