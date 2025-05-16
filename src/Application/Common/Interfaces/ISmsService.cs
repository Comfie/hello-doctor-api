using Ardalis.Result;
using HelloDoctorApi.Application.Common.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Common.Interfaces
{
    public interface ISmsService
    {
        public Task<Result<bool>> SendSmsOtpAsync(string userId, string phoneNumber,
            CancellationToken cancellationToken = default);

        public Task<Result<bool>> ConfirmVerification(string userId, string otp,
            CancellationToken cancellationToken = default);

        //Task<ResponseStatusEnum> SendConfirmationSMS(string senderPhone, string message);
        //Task<ResponseStatusEnum> SendDispatchSMS(string senderPhone, string message);
        //Task<ResponseStatusEnum> PackageDeliveredSms(string senderPhone, string message);
        //Task<ResponseStatusEnum> ReceiverDeliverySms(string senderPhone, string message);
        //Task<ResponseStatusEnum> ReceiverPackageDeliveredSms(string senderPhone, string message);
    }
}