using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Common;

namespace HelloDoctorApi.Domain.Entities;

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