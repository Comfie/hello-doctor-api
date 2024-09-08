using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Entities;

public class Beneficiary : BaseAuditableEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string EmailAddress { get; set; }
    public RelationshipToMainMember Relationship { get; set; }
    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }
    public bool IsDeleted { get; set; }
}