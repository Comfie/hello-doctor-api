using Ardalis.Result;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Updates.DeletePharmacy;

public record DeletePharmacyCommand(long Id) : IRequest<Result<bool>>;