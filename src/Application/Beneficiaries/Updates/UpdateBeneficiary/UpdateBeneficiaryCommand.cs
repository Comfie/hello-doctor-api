using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.UpdateBeneficiary;

public record UpdateBeneficiaryCommand(
    long Id,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? EmailAddress,
    string? Gender,
    DateTime? DateOfBirth,
    string? RelationshipToMainMember) : IRequest<Result<BeneficiaryResponse>>;