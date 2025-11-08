using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public required string AccountId { get; set; }
    public required ApplicationUser Account { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    public required string PrimaryContact { get; set; }
    public string? SecondaryContact { get; set; }
    public string? QualificationDescription { get; set; }
    public bool IsActive { get; set; }
    public string? Speciality { get; set; }
    public ICollection<Pharmacy>? Pharmacies { get; set; } = new List<Pharmacy>();
}