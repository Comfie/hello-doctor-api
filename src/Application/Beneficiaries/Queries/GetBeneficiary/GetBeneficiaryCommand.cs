using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiary;

public record GetBeneficiaryCommand(long Id) : IRequest<Result<BeneficiaryResponse>>;