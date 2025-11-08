using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetMyPharmacy;

public record GetMyPharmacyQuery : IRequest<Result<PharmacyResponse>>;
