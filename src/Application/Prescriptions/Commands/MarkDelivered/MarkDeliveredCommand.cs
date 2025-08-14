using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Commands.MarkDelivered;

public record MarkDeliveredCommand(long PrescriptionId) : IRequest<Result<bool>>;
