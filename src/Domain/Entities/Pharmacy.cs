namespace HelloDoctorApi.Domain.Entities;

public class Pharmacy : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ContactNumber { get; set; }
    public required string ContactEmail { get; set; }
    public required string ContactPerson { get; set; }
    public string? Logo { get; set; }
    public required string Address { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Pharmacist>? Pharmacists { get; set; } = new List<Pharmacist>();
    public ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();
    
    //it needs to be linked with a list of doctors
    //also 
}