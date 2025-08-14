using Ardalis.Result;
using HelloDoctorApi.Application.Prescriptions.Models;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IUploadPrescriptionService
{
    Task<Result<long>> UploadAsync(UploadPrescriptionDto form, CancellationToken cancellationToken);
}