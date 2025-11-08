using Ardalis.Result;
using HelloDoctorApi.Application.Pharmacies.Models;

namespace HelloDoctorApi.Application.Pharmacies.Updates.UpdatePharmacy;

public record UpdatePharmacyCommand(
    long Id,
    string? Name,
    string? Description,
    string? ContactNumber,
    string? ContactEmail,
    string? ContactPerson,
    string? Address,
    TimeSpan? OpeningTime,
    TimeSpan? ClosingTime) : IRequest<Result<PharmacyResponse>>;