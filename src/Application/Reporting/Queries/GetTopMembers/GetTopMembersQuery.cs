using Ardalis.Result;
using HelloDoctorApi.Application.Reporting.Models;

namespace HelloDoctorApi.Application.Reporting.Queries.GetTopMembers;

public record GetTopMembersQuery(long PharmacyId, int TopCount = 10, int DaysBack = 30) : IRequest<Result<List<TopMemberResponse>>>;
