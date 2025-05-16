using Ardalis.Result;
using HelloDoctorApi.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace HelloDoctorApi.Application.Pharmacies.Commands.UploadPharmacyLogo;

public record UploadPharmacyLogoCommand(long Id, IFormFile File) : IRequest<Result<bool>>;