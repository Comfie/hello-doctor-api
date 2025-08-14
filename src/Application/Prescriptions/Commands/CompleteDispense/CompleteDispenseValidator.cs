using FluentValidation;

namespace HelloDoctorApi.Application.Prescriptions.Commands.CompleteDispense;

public class CompleteDispenseValidator : AbstractValidator<CompleteDispenseCommand>
{
    public CompleteDispenseValidator()
    {
        RuleFor(x => x.PrescriptionId).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(1000);
    }
}
