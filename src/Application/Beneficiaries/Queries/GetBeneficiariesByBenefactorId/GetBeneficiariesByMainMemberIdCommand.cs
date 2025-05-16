using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiariesByBenefactorId;

public record GetBeneficiariesByMainMemberIdCommand(string MainMemberId) : IRequest<Result<List<BeneficiaryResponse>>>;