using Ardalis.Result;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.MainMembers.Queries.GetMainMembers;

public record GetMainMembersQuery() : IRequest<Result<List<MainMemberResponse>>>
{
}