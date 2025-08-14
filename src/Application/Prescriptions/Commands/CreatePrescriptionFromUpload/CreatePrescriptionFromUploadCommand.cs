using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Commands.CreatePrescriptionFromUpload;

public record CreatePrescriptionFromUploadCommand(long BeneficiaryId, long FileUploadId, long? PharmacyId = null, string? Notes = null)
    : IRequest<Result<long>>;
