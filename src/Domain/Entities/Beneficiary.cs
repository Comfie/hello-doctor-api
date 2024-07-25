using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Domain.Entities;

public class Beneficiary : BaseAuditableEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string EmailAddress { get; set; }
    public RelationshipToBenefactor Relationship { get; set; }
    public required string BenefactorId { get; set; }
    public required ApplicationUser Benefactor { get; set; }
    public bool IsDeleted { get; set; }
}