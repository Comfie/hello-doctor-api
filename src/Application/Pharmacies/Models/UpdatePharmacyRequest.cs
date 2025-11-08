namespace HelloDoctorApi.Application.Pharmacies.Models;

public class UpdatePharmacyRequest
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ContactNumber { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
}