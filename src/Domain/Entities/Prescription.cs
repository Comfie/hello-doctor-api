using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Domain.Entities;

public class Prescription : BaseAuditableEntity
{
    public string? Notes { get; set; }
    public required string Code { get; set; }
    public string? Logo { get; set; }
    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }
    public long BeneficiaryId { get; set; }
    public required Beneficiary Beneficiary { get; set; }
}