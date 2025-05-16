using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.UpdateBeneficiary;

public record UpdateBeneficiaryCommand(
    long Id,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? EmailAddress) : IRequest<Result<BeneficiaryResponse>>;