using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Commands.UploadPrescription;

public record UploadPrescriptionCommand(long BeneficiaryId, long? PharmacyId = null, string? Notes = null)
    : IRequest<Result<long>>;