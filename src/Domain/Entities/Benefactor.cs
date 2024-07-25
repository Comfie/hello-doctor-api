using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Domain.Entities;

public class Benefactor : BaseAuditableEntity
{
    public required string AccountId { get; set; }
    public required ApplicationUser Account { get; set; }
    public ICollection<Beneficiary>? Beneficiaries { get; set; }
}