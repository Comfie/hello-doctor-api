using Ardalis.Result;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.MainMembers.Queries.GetMainMemberById;

public record GetMainMemberByIdQuery(long Id) : IRequest<Result<MainMemberResponse>>
{
}