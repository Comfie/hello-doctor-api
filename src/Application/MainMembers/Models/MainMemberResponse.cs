namespace HelloDoctorApi.Application.MainMembers.Models;

public class MainMemberResponse
{
    public required long Id { get; set; }
    public required string MemberShipNumber { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public int? NumberOfBeneficiaries { get; set; } = 0;
}