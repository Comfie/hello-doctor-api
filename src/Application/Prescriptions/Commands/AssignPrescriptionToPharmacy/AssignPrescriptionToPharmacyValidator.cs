using FluentValidation;

namespace HelloDoctorApi.Application.Prescriptions.Commands.AssignPrescriptionToPharmacy;

public class AssignPrescriptionToPharmacyValidator : AbstractValidator<AssignPrescriptionToPharmacyCommand>
{
    public AssignPrescriptionToPharmacyValidator()
    {
        RuleFor(x => x.PrescriptionId).GreaterThan(0);
        RuleFor(x => x.PharmacyId).GreaterThan(0);
    }
}
