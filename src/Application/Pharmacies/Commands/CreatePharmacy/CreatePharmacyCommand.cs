using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Commands.CreatePharmacy;

public record CreatePharmacyCommand(CreatePharmacyRequest Request) : IRequest<Result<PharmacyResponse>>;