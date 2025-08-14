using FluentValidation;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetMyPrescriptions;

public class GetMyPrescriptionsValidator : AbstractValidator<GetMyPrescriptionsQuery>
{
    public GetMyPrescriptionsValidator()
    {
        RuleFor(x => x.BeneficiaryCode).MaximumLength(32);
    }
}

