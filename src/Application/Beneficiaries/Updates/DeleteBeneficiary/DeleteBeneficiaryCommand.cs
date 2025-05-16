using Ardalis.Result;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.DeleteBeneficiary;

public record DeleteBeneficiaryCommand(long Id) : IRequest<Result<bool>>
{
}