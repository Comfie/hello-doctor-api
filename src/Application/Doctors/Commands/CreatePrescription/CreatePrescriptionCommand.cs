using Ardalis.Result;

namespace HelloDoctorApi.Application.Doctors.Commands.CreatePrescription;

public record CreatePrescriptionCommand(
    long BeneficiaryId,
    DateTimeOffset IssuedDate,
    DateTimeOffset ExpiryDate,
    string? Notes = null,
    long? PharmacyId = null
) : IRequest<Result<long>>;
