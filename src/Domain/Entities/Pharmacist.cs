using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Domain.Entities;

public class Pharmacist : BaseAuditableEntity
{
    
    public required string AccountId { get; set; }
    public required ApplicationUser Account { get; set; }
    
    public long PharmacyId { get; set; }
    public required Pharmacy Pharmacy { get; set; }
    
}