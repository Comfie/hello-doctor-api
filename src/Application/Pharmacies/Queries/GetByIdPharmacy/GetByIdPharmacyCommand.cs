using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetByIdPharmacy;

public record GetByIdPharmacyCommand(long Id) : IRequest<Result<PharmacyResponse>>;