using System.Net.Http.Headers;
using System.Text;
using System.Web;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Common.Models;
using ApiBaseTemplate.Application.Common.Models.Settings;
using ApiBaseTemplate.Domain.Entities.Auth;
using ApiBaseTemplate.Domain.Shared;
using ApiBaseTemplate.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OtpNet;

namespace ApiBaseTemplate.Web.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly SmsSettings _smsSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDateTimeService _dateTime;
        private readonly HttpClient _httpClient;

        public SmsService(ILogger<SmsService> logger,
            IOptions<SmsSettings> smsSettings,
            ApplicationDbContext applicationDbContext,
            IDateTimeService dateTime, UserManager<ApplicationUser> userManager,
            HttpClient httpClient)
        {
            _logger = logger;
            _smsSettings = smsSettings.Value;
            _applicationDbContext = applicationDbContext;
            _dateTime = dateTime;
            _userManager = userManager;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_smsSettings.BaseAddress ?? "");
        }

        public async Task<Result<CustomResponse>> SendSmsOtpAsync(string userId, string phoneNumber,
      CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return new CustomResponse(message: "User not found", success: false);

            var formattedPhoneNumber = phoneNumber.Replace("+", "").Replace(" ", "");

            if (user.PhoneNumber is null)
            {
                var checkPhoneNumber = _applicationDbContext
                    .ApplicationUsers
                    .SingleOrDefaultAsync(x => x.PhoneNumber == formattedPhoneNumber, cancellationToken: cancellationToken);

                if (checkPhoneNumber is not null)
                    return new CustomResponse(message: "Phone number already taken", success: false);

                user.PhoneNumber = formattedPhoneNumber;
                await _userManager.UpdateAsync(user);
            }
            else if (!user.PhoneNumber.Equals(formattedPhoneNumber, StringComparison.OrdinalIgnoreCase))
            {
                user.PhoneNumber = formattedPhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            var utf8 = new UTF8Encoding();
            byte[] secret = utf8.GetBytes(user.Id);
            var hotp = new Hotp(secret, hotpSize: 6);
            var otp = await _applicationDbContext
            .OneTimePins
            .FirstOrDefaultAsync(x => x.UserId == userId,
                    cancellationToken: cancellationToken);

            if (otp is null)
            {
                otp = new OneTimePin { UserId = user.Id, User = user, Counter = 1, LastIssuedAt = _dateTime.Now };
                _applicationDbContext.OneTimePins.Add(otp);
            }
            else
            {
                if ((_dateTime.Now - otp.LastIssuedAt).TotalSeconds < 30)
                {
                    return new CustomResponse(message: "OTP already sent", success: false);
                }

                otp.Counter += 1;
                otp.LastIssuedAt = _dateTime.Now;
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            var message = $"This is your One Time Password: {hotp.ComputeHOTP(otp.Counter)} from Admin. For security reasons, do not share this code.";

            var result = await SendSms(formattedPhoneNumber, message, cancellationToken);
            return result
                ? Result.Success(new CustomResponse(message: "OTP sent successfully", success: true))
                : Result.Failure<CustomResponse>(new Error(message: "Failed to send OTP", code:"OTP_SEND_FAILED"));
        }

        public async Task<bool> ConfirmVerification(string userId, string otp,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var oneTimePin = await _applicationDbContext
                .OneTimePins
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Counter)
                .FirstOrDefaultAsync(cancellationToken);

            if (oneTimePin is null || user is null)
            {
                return false;
            }

            var utf8 = new UTF8Encoding();
            byte[] secret = utf8.GetBytes(userId);
            var hotp = new Hotp(secret);
            var verifiedOtp = hotp.VerifyHotp(otp, oneTimePin.Counter);
            oneTimePin.Counter += 1;

            if (verifiedOtp)
            {
                user.IsActive = true;
                user.PhoneNumberConfirmed = true;
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return verifiedOtp;
        }

        private async Task<bool> SendSms(string phoneNumber, string message,
            CancellationToken cancellationToken = default)
        {
            String encodeMessage = HttpUtility.UrlEncode(message);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestUrl =
                $"/json?action=message_send&username={_smsSettings.Username}&password={_smsSettings.Password}&to={phoneNumber}&text={encodeMessage}&from={_smsSettings.From}";

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                ApiResponse? apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                if (apiResponse!.Status == 1)
                    return true;
            }

            return false;
        }
    }

    public class ApiResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
    }
}