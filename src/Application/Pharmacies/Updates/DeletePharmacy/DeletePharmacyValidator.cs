namespace HelloDoctorApi.Application.Pharmacies.Updates.DeletePharmacy;

public class DeletePharmacyValidator : AbstractValidator<DeletePharmacyCommand>
{
    public DeletePharmacyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}