using Ardalis.Result;
using HelloDoctorApi.Application.MainMembers.Models;

namespace HelloDoctorApi.Application.MainMembers.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<Result<MainMemberProfileResponse>>;
