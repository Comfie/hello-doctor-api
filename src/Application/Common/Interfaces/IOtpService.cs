using Ardalis.Result;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IOtpService
{
    Task<Result<bool>> SendOtpAsync(string userId, string phoneNumber, CancellationToken cancellationToken = default);
    Task<Result<bool>> VerifyOtpAsync(string userId, string otp, CancellationToken cancellationToken = default);
}