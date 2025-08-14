using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Domain.Entities;

public class MainMember : BaseAuditableEntity
{
    public required string Code { get; set; }
    public required string AccountId { get; set; }
    public required ApplicationUser Account { get; set; }
    public long? DefaultPharmacyId { get; set; }
    public ICollection<Beneficiary>? Beneficiaries { get; set; }
}