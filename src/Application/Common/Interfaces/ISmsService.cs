using ApiBaseTemplate.Application.Common.Models;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Common.Interfaces
{
    public interface ISmsService 
    {
        public Task<Result<CustomResponse>> SendSmsOtpAsync(string userId, string phoneNumber, CancellationToken cancellationToken = default);

        public Task<bool> ConfirmVerification(string userId, string otp,
            CancellationToken cancellationToken = default);

        //Task<ResponseStatusEnum> SendConfirmationSMS(string senderPhone, string message);
        //Task<ResponseStatusEnum> SendDispatchSMS(string senderPhone, string message);
        //Task<ResponseStatusEnum> PackageDeliveredSms(string senderPhone, string message);
        //Task<ResponseStatusEnum> ReceiverDeliverySms(string senderPhone, string message);
        //Task<ResponseStatusEnum> ReceiverPackageDeliveredSms(string senderPhone, string message);
    }
}