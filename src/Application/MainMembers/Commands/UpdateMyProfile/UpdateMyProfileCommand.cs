using Ardalis.Result;
using HelloDoctorApi.Application.MainMembers.Models;

namespace HelloDoctorApi.Application.MainMembers.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    long? DefaultPharmacyId
) : IRequest<Result<MainMemberProfileResponse>>;
