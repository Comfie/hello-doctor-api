namespace HelloDoctorApi.Application.Beneficiaries.Models;

public record CreateBeneficiaryRequest(
    string MainMemberId,
    string LastName,
    string PhoneNumber,
    string FirstName,
    string EmailAddress,
    string Relationship);