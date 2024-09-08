namespace HelloDoctorApi.Domain.Entities.Auth;

public class SystemAdministrator : BaseAuditableEntity
{
    public required string UserId { get; set; }
    public required ApplicationUser User { get; set; }
}