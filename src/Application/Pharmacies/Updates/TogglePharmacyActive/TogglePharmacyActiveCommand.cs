using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;

namespace HelloDoctorApi.Application.Pharmacies.Updates.TogglePharmacyActive;

public record TogglePharmacyActiveCommand(long Id) : IRequest<Result<PharmacyResponse>>;
