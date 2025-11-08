namespace HelloDoctorApi.Domain.Entities.Auth;

public class SystemAdministrator : BaseAuditableEntity
{
    public required string UserId { get; set; }
    public required ApplicationUser User { get; set; }

    public long? PharmacyId { get; set; }
    public Pharmacy? Pharmacy { get; set; }
}