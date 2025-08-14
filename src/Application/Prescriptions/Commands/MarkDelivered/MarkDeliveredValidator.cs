using FluentValidation;

namespace HelloDoctorApi.Application.Prescriptions.Commands.MarkDelivered;

public class MarkDeliveredValidator : AbstractValidator<MarkDeliveredCommand>
{
    public MarkDeliveredValidator()
    {
        RuleFor(x => x.PrescriptionId).GreaterThan(0);
    }
}
