using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Commands.CreateBeneficiary;

public record CreateBeneficiaryCommand(CreateBeneficiaryRequest Request) : IRequest<Result<long>>;