namespace HelloDoctorApi.Application.Beneficiaries.Models;

public class BeneficiaryResponse
{
    public long Id { get; set; }
    public string? MainMemberName { get; set; }
    public string? MainMemberContact { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? MainMemberId { get; set; }
    public string? RelationshipToMainMember { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? BeneficiaryCode { get; set; }
    

    public BeneficiaryResponse(long id, string? mainMemberName, string? mainMemberContact, string firstName,
        string? lastName, string? phoneNumber, string? emailAddress, string? mainMemberId,
        string? relationshipToMainMember, string? gender, DateTime? dateOfBirth, string? beneficiaryCode)
    {
        Id = id;
        MainMemberName = mainMemberName;
        MainMemberContact = mainMemberContact;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
        MainMemberId = mainMemberId;
        RelationshipToMainMember = relationshipToMainMember;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        BeneficiaryCode = beneficiaryCode;
    }
}