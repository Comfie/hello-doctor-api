using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Commands.AssignPrescriptionToPharmacy;

public record AssignPrescriptionToPharmacyCommand(long PrescriptionId, long PharmacyId) : IRequest<Result<bool>>;
