using System.Text;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using HelloDoctorApi.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OtpNet;

namespace HelloDoctorApi.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTime;

    public OtpService(
        ApplicationDbContext applicationDbContext,
        UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IDateTimeService dateTime)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }


    public async Task<Result<bool>> SendOtpAsync(string userId, string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<bool>(new Error("Send Otp", "User not found"));

        var formattedPhoneNumber = phoneNumber.Replace("+", "").Replace(" ", "");

        var utf8 = new UTF8Encoding();
        byte[] secret = utf8.GetBytes(user.Id);
        var hotp = new Hotp(secret, hotpSize: 6);
        var otp = await _applicationDbContext
            .OneTimePins
            .FirstOrDefaultAsync(x => x.UserId == userId,
                cancellationToken: cancellationToken);

        if (otp is null)
        {
            otp = new OneTimePin { 
                UserId = user.Id, 
                User = user, 
                Counter = 1, 
                LastIssuedAt = _dateTime.Now 
            };
            await _applicationDbContext.OneTimePins.AddAsync(otp, cancellationToken);
        }
        else
        {
            if ((_dateTime.Now - otp.LastIssuedAt).TotalSeconds < 30)
            {
                return Result.Failure<bool>(new Error("Send Otp", "Please try again after 30 seconds"));
            }

            otp.Counter += 1;
            otp.LastIssuedAt = _dateTime.Now;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var message =
            $"This is your One Time Password: {hotp.ComputeHOTP(otp.Counter)} from Admin. For security reasons, do not share this code.";
        
        return Result.Success(true);
    }

    public async Task<Result<bool>> VerifyOtpAsync(string userId, string otp,
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
}