using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiaries;

public record GetBeneficiariesCommand : IRequest<Result<List<BeneficiaryResponse>>>
{
}