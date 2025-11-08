namespace HelloDoctorApi.Application.MainMembers.Models;

public class MainMemberProfileResponse
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Code { get; set; }
    public long? DefaultPharmacyId { get; set; }
}
