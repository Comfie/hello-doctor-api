using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Infrastructure.Data.Interceptors.Interfaces;

namespace HelloDoctorApi.Domain.Entities;

public class Beneficiary : BaseAuditableEntity, ITimestamped, ISoftDelete
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string EmailAddress { get; set; }
    public required string BeneficiaryCode { get; set; }
    public RelationshipToMainMember Relationship { get; set; }
    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}