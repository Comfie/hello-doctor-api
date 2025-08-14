using Ardalis.Result;

namespace HelloDoctorApi.Application.MainMembers.Commands.UpdatePreferences;

public record UpdateMainMemberPreferencesCommand(long? DefaultPharmacyId) : IRequest<Result<bool>>;

