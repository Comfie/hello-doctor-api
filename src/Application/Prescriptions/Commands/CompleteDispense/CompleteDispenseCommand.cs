using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Commands.CompleteDispense;

public record CompleteDispenseCommand(long PrescriptionId, bool IsPartial, string? Note) : IRequest<Result<bool>>;
