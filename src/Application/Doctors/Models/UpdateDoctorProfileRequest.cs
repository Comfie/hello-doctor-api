namespace HelloDoctorApi.Application.Doctors.Models;

public class UpdateDoctorProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? PrimaryContact { get; set; }
    public string? SecondaryContact { get; set; }
    public string? QualificationDescription { get; set; }
    public string? Speciality { get; set; }
}
