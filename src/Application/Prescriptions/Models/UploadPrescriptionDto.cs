using Microsoft.AspNetCore.Http;

namespace HelloDoctorApi.Application.Prescriptions.Models;

public class UploadPrescriptionDto
{
    public long BeneficiaryId { get; set; }
    public IFormFile File { get; set; } = default!;
    public long? PharmacyId { get; set; }
    public string? Notes { get; set; }
}
