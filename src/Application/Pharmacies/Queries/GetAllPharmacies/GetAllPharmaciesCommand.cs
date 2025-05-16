using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetAllPharmacies;

public record GetAllPharmaciesCommand : IRequest<Result<List<PharmacyResponse>>>;