using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetActivePharmacies;

public record GetActivePharmaciesCommand : IRequest<Result<List<PharmacyResponse>>>;