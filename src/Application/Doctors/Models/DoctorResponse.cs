namespace HelloDoctorApi.Application.Doctors.Models;

public record DoctorResponse(
    long Id,
    string FirstName,
    string LastName,
    string EmailAddress,
    string PrimaryContact,
    string? SecondaryContact,
    string? QualificationDescription,
    string? Speciality,
    bool IsActive,
    List<string>? AssociatedPharmacies
);
