using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Common.Models;
using HelloDoctorApi.Application.Common.Models.Settings;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using HelloDoctorApi.Infrastructure.Data;
using HelloDoctorApi.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OtpNet;

namespace HelloDoctorApi.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeService _dateTime;
    private readonly ApplicationDbContext _context;
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMainMemberService _mainMemberService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService, IDateTimeService dateTime,
        ApplicationDbContext context,
        IOptions<AppSettings> appSettings,
        IEmailService emailService, IUnitOfWork unitOfWork,
        IMainMemberService mainMemberService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _dateTime = dateTime;
        _context = context;
        _appSettings = appSettings.Value;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _mainMemberService = mainMemberService;
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken);

        return user.UserName;
    }

    public async Task<Result<AuthResponse>> AuthenticateUserAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return Result<AuthResponse>.NotFound(new Error("Getting User", "User not found"));

            var refreshToken = _jwtService.GenerateRefreshToken(user);

            var signingCredentials = _jwtService.GetSigningCredentials();
            var claims = await _jwtService.GetClaims(user);
            var tokenOptions = _jwtService.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var userRoles = await _userManager.GetRolesAsync(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = _dateTime.OffsetNow.AddDays(7);

            var authResponse = new AuthResponse
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault() ?? string.Empty,
                JwtToken = token,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = user.RefreshTokenExpiryTime
            };
            await _userManager.UpdateAsync(user);
            return Result<AuthResponse>.Success(authResponse);
        }

        return Result<AuthResponse>.NotFound(new Error("Authenticate User", "User not found"));
    }

    public async Task<Result<bool>> CreateUserAsync(CreateUserRequest createUserRequest,
        CancellationToken cancellationToken = default)
    {
        var checkUser = await _userManager.FindByEmailAsync(createUserRequest.Email);

        if (checkUser is not null)

            return Result<bool>.Error(new Error("Creating User", "User with this email already exists"));

        var role = "User";

        var applicationUser = new ApplicationUser
        {
            UserName = createUserRequest.Email,
            FirstName = createUserRequest.Firstname,
            LastName = createUserRequest.Lastname,
            PhoneNumber = createUserRequest.PhoneNumber,
            Email = createUserRequest.Email,
            CreatedDate = _dateTime.OffsetNow,
            IsActive = false
        };

        var result = await _userManager.CreateAsync(applicationUser, createUserRequest.Password);

        if (await _roleManager.RoleExistsAsync(role))
        {
            await _userManager.AddToRoleAsync(applicationUser, role);
            await CreateUserTypeBasedOnRole(cancellationToken, role, applicationUser);
        }

        if (!result.Succeeded)
            return Result<bool>.Error(new Error("Creating User", "Failed to create user"));

        //send email to system admin to activate account 

        return Result.Success<bool>(true);
    }

    private async Task CreateUserTypeBasedOnRole(CancellationToken cancellationToken, string role,
        ApplicationUser applicationUser)
    {
        switch (role)
        {
            case nameof(UserType.SuperAdministrator):
                var superAdmin = new SuperAdministrator()
                {
                    UserId = applicationUser.Id,
                    User = applicationUser
                };
                _context.SuperAdministrators.Add(superAdmin);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            case nameof(UserType.SystemAdministrator):
                var systemAdministrator = new SystemAdministrator()
                {
                    UserId = applicationUser.Id,
                    User = applicationUser
                };
                _context.SystemAdministrators.Add(systemAdministrator);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            case nameof(UserType.MainMember):
                var mainMember = new MainMember
                {
                    Code = await _mainMemberService.GenerateUniqueMemberNumberAsync(cancellationToken),
                    AccountId = applicationUser.Id,
                    Account = applicationUser,
                };
                _context.MainMembers.Add(mainMember);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            case nameof(UserType.Pharmacist):
                var getPharmacy =
                    await _context.Pharmacies.FirstOrDefaultAsync(pharmacy => pharmacy.Id == 1, cancellationToken);
                if (getPharmacy is not null)
                {
                    var pharmacist = new Pharmacist()
                    {
                        Pharmacy = getPharmacy,
                        PharmacyId = getPharmacy.Id,
                        AccountId = applicationUser.Id,
                        Account = applicationUser,
                    };
                    _context.Pharmacists.Add(pharmacist);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                break;
            case nameof(UserType.Doctor):
                var getDoctor =
                    await _context.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == 1, cancellationToken);
                if (getDoctor is not null)
                {
                    var doctor = new Doctor
                    {
                        FirstName = applicationUser.FirstName ?? String.Empty,
                        LastName = applicationUser.LastName ?? String.Empty,
                        EmailAddress = applicationUser.Email ?? String.Empty,
                        PrimaryContact = applicationUser.PhoneNumber ?? String.Empty,
                    };
                    _context.Doctors.Add(doctor);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                break;
        }
    }

    public async Task<Result<bool>> IsInRoleAsync(string userId, string role,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result<bool>.Error(new Error("Checking Role", "User not found"));

        var result = await _userManager.IsInRoleAsync(user, role);

        return Result.Success(result);
    }

    public async Task<Result<List<string>>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result<List<string>>.Error(new Error("Getting User Roles", "User not found"));

        var roles = await _userManager.GetRolesAsync(user); 

        return Result.Success(roles.ToList());
    }

    public async Task<Result<bool>> UpdateRoleAsync(string userId, string role,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result<bool>.Error(new Error("Updating Role", "User not found"));

        // Remove existing roles
        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);

        // Add the new role
        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            Result<bool>.Error(new Error("Updating Role", "Failed to update user role"));
        }

        // Save changes
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            Result<bool>.Error(new Error("Updating Role", "Failed to update user role"));
        }

        return Result.Success(true);
    }

    public async Task<Result<bool>> AuthorizeAsync(string userId, string policyName,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result<bool>.Error(new Error("Authorize User", "User not found"));

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return Result.Success(result.Succeeded);
    }

    public async Task<Result<string>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result<String>.NotFound(new Error("Delete User", "User not found"));
        
        user.IsDeleted = true;
        user.DeletedAt = _dateTime.OffsetNow;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success<string>("User deleted successfully")
            : Result<String>.Error(new Error("Delete User",
                result.Errors.Select(e => e.Description).ToString() ?? "Failed to delete user"));
    }

    public async Task<Result<bool>> CreateRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var result = await _roleManager.CreateAsync(new ApplicationRole { Name = role });
        return result.Succeeded
            ? Result.Success<bool>(true)
            : Result<bool>.Error(new Error("Create Role",
                result.Errors.Select(e => e.Description).ToString() ?? "Failed to create role"));
    }

    public async Task<Result<bool>> DeleteRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var roleToDelete = await _roleManager.FindByNameAsync(role);
        if (roleToDelete == null)
        {
            return Result<bool>.Error(new Error("Delete Role", "Role not found"));
        }

        if (roleToDelete is { } applicationRole)
        {
            applicationRole.IsDeleted = true;
            applicationRole.DeletedAt = _dateTime.OffsetNow;

            var result = await _roleManager.UpdateAsync(applicationRole);
            return result.Succeeded
                ? Result.Success(true)
                : Result<bool>.Error(new Error("Delete Role",
                    result.Errors.Select(e => e.Description).ToString() ?? "Failed to delete role"));
        }

        return Result<bool>.Error(new Error("Delete Role", "Invalid role type"));
    }

    public async Task<Result<bool>> UpdateRoleStatusAsync(string role, CancellationToken cancellationToken = default)
    {
        var roleToUpdate = await _roleManager.FindByNameAsync(role);
        if (roleToUpdate == null)
        {
            return Result<bool>.Error(new Error("Update Role", "Role not found"));
        }

        if (roleToUpdate is ApplicationRole applicationRole)
        {
            applicationRole.IsDeleted = false;
            applicationRole.DeletedAt = null;
            applicationRole.UpdatedAt = _dateTime.OffsetNow;

            var result = await _roleManager.UpdateAsync(applicationRole);
            return result.Succeeded
                ? Result.Success(true)
                : Result<bool>.Error(new Error("Update Role",
                    result.Errors.Select(e => e.Description).ToString() ?? "Failed to update role"));   
        }

        return Result<bool>.Error(new Error("Update Role", "Invalid role type"));
    }

    public async Task<Result<string>> DeleteUserAsync(ApplicationUser user)
    {
      //soft delete
      var userDetails = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == user.Id);

        if (userDetails is null)
            return Result<String>.NotFound(new Error("Delete User", "User not found"));

        userDetails.IsDeleted = true;
        userDetails.DeletedAt = DateTimeOffset.UtcNow;

        var result = await _userManager.UpdateAsync(userDetails);

        return result.Succeeded
            ? Result.Success<string>("User deleted successfully")
            : Result<String>.Error(new Error("Delete User",
                result.Errors.Select(e => e.Description).ToString() ?? "Failed to delete user"));
    }

    public async Task<Result<UserDetailsResponse>> GetUserById(string userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _userManager.FindByIdAsync(userId);

        if (account is null)
            return Result<UserDetailsResponse>.NotFound(new Error("Get User Error", "User not found"));


        var user = new UserDetailsResponse()
        {
            Id = account.Id,
            Username = account.UserName,
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName,
            PhoneNumber = account.PhoneNumber
        };

        var userRoles = await _userManager.GetRolesAsync(account);
        user.Role = userRoles.FirstOrDefault() ?? string.Empty;

        return Result.Success(user);
    }

    public async Task<Result<bool>> RevokeRoleAsync(string userId, string role,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result<bool>.Error(new Error("Revoke Role", "User not found"));

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            return Result<bool>.Error(new Error("Revoke Role", "User not in role found"));
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
        {
            return Result<bool>.Error(new Error("Revoke Role", "Unable to remove user from role"));
        }

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return Result<bool>.Error(new Error("Revoke Role", "Failed to update user"));
        }

        return Result.Success(true);
    }

    public async Task<Result<List<UserDetailsResponse>>> GetUsers(CancellationToken cancellationToken = default)
    {
        // Fetch users and their roles in a single query using a join
        var userRoles = await (from user in _context.ApplicationUsers.IncludeSoftDeleted()
            join userRole in _context.UserRoles on user.Id equals userRole.UserId
            join role in _context.Roles on userRole.RoleId equals role.Id
            select new
            {
                user.Email,
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                RoleName = role.Name
            }).ToListAsync(cancellationToken);

        // Group the results by user and aggregate roles
        var groupedUsers = userRoles
            .GroupBy(u => new { u.Email, u.Id, u.UserName, u.FirstName, u.LastName, u.PhoneNumber })
            .Select(g => new UserDetailsResponse
            {
                Email = g.Key.Email,
                Id = g.Key.Id,
                Username = g.Key.UserName,
                FirstName = g.Key.FirstName,
                LastName = g.Key.LastName,
                Role = string.Join(",", g.Select(u => u.RoleName)),
                PhoneNumber = g.Key.PhoneNumber
            }).ToList();


        return Result.Success(groupedUsers);
    }

    public async Task<Result<List<UserRoleResponse>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _context
            .ApplicationRoles
            .Where(r => !r.IsDeleted)
            .Select(roles => new UserRoleResponse
            {
                Id = roles.Id,
                IsDeleted = roles.IsDeleted,
                Description = roles.Description,
                RoleName = roles.Name ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return Result.Success(roles);
    }

    public async Task<Result<List<UserDetailsResponse>>> GetActiveUsers(CancellationToken cancellationToken = default)
    {
        // Fetch users and their roles in a single query using a join
        var userRoles = await (from user in _context.ApplicationUsers
            join userRole in _context.UserRoles on user.Id equals userRole.UserId
            join role in _context.Roles on userRole.RoleId equals role.Id
            where user.IsActive
            select new
            {
                user.Email,
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                RoleName = role.Name
            }).ToListAsync(cancellationToken);

        // Group the results by user and aggregate roles
        var groupedUsers = userRoles
            .GroupBy(u => new { u.Email, u.Id, u.UserName, u.FirstName, u.LastName, u.PhoneNumber })
            .Select(g => new UserDetailsResponse
            {
                Email = g.Key.Email,
                Id = g.Key.Id,
                Username = g.Key.UserName,
                FirstName = g.Key.FirstName,
                LastName = g.Key.LastName,
                Role = string.Join(",", g.Select(u => u.RoleName)),
                PhoneNumber = g.Key.PhoneNumber
            }).ToList();

        return Result.Success(groupedUsers);
    }


    public async Task<Result<UserDetailsResponse>> UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<UserDetailsResponse>.NotFound(new Error("Update User", "User not found"));

        user.FirstName = updateUserRequest.FirstName;
        user.LastName = updateUserRequest.LastName;
        user.Email = updateUserRequest.Email;
        user.PhoneNumber = updateUserRequest.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result<UserDetailsResponse>.Error(new Error("Update User", "Failed to update user"));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userDetails = new UserDetailsResponse
        {
            Email = user.Email,
            Id = user.Id,
            FirstName = updateUserRequest.FirstName,
            LastName = updateUserRequest.LastName,
            Username = user.UserName,
            Role = roles.FirstOrDefault(),
            PhoneNumber = user.PhoneNumber
        };

        return Result.Success(userDetails);
    }

    public async Task<Result<bool>> ResetPasswordAsync(UpdateUserPasswordRequest updateUserPasswordRequest,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(updateUserPasswordRequest.Email);
        if (user is null)
        {
            return Result<bool>.NotFound(new Error("Reset Password", "User not found"));
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, resetToken, updateUserPasswordRequest.Password);
        if (!result.Succeeded)
        {
            return Result<bool>.Error(new Error("Reset Password", "Failed to reset password"));
        }

        return Result.Success(true);
    }

    public async Task<Result<bool>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<bool>.NotFound(new Error("Forgot Password", "User not found"));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        string url = $"{_appSettings.WebUrl}/auth/reset-password";
        string resetPasswordUrl = $"<a href=\"{url}\" target=\"_blank\"> Reset Password </a>";

        var emailBody = $@"<h3><b>Hi {user.FirstName}</b></h3>
                        <p>You have requested to reset your password.</p>
                        <p>Click on the link to reset your password {resetPasswordUrl}</p>
                        <p>Thank you.</p>";

        var param = new Dictionary<string, string?>
        {
            { "token", token },
            { "email", email }
        };

        //still needs to be tested

        var callback = QueryHelpers.AddQueryString(emailBody, param);
        var message = new Message(new string[] { user.Email ?? string.Empty }, "Reset password token", callback, null);

        var result = await _emailService.SendEmailAsync(message, cancellationToken);

        return Result.Success(result);
    }

    public async Task<Result<bool>> ConfirmVerification(string userId, string otp,
        CancellationToken cancellationToken = default)
    {
        var oneTimePin = await _context.OneTimePins
            .Where(x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oneTimePin is null)
        {
            return Result<bool>.NotFound(new Error("Confirm Verification", "OTP code not found"));
        }

        var utf8 = new UTF8Encoding();
        byte[] secret = utf8.GetBytes(userId);
        var hotp = new Hotp(secret);
        var verifiedOtp = hotp.VerifyHotp(otp, oneTimePin.Counter);
        oneTimePin.Counter += 1;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(verifiedOtp);
    }
}