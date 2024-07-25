using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public required string PrimaryContact { get; set; }
    public string? SecondaryContact { get; set; }
    public string? QualificationDescription { get; set; }
    public required string AccountId { get; set; }
    public required ApplicationUser Account { get; set; }
}