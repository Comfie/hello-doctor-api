using FluentValidation;

namespace HelloDoctorApi.Application.MainMembers.Commands.UpdatePreferences;

public class UpdateMainMemberPreferencesValidator : AbstractValidator<UpdateMainMemberPreferencesCommand>
{
    public UpdateMainMemberPreferencesValidator()
    {
        // optional, but if provided must be positive
        RuleFor(x => x.DefaultPharmacyId).GreaterThan(0).When(x => x.DefaultPharmacyId.HasValue);
    }
}

