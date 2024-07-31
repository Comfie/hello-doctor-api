using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    public required string PrimaryContact { get; set; }
    public string? SecondaryContact { get; set; }
    public string? QualificationDescription { get; set; }
    public bool IsActive { get; set; }
}