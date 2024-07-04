using ApiBaseTemplate.Application.Authentications.Models;
using ApiBaseTemplate.Application.Common.Models;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<Result<UserDetailsResponse>> GetUserById(string userId, CancellationToken cancellationToken = default);

    Task<Result<List<UserDetailsResponse>>> GetUsers(CancellationToken cancellationToken = default);

    Task<Result<List<UserDetailsResponse>>> GetActiveUsers(CancellationToken cancellationToken = default);

    Task<Result<UserDetailsResponse>> UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest, CancellationToken cancellationToken = default);

    Task<Result<bool>> ResetPasswordAsync(UpdateUserPasswordRequest updateUserPasswordRequest, CancellationToken cancellationToken = default);
    Task<Result<bool>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

    Task<Result<bool>> ConfirmVerification(string userId, string otp, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    Task<Result<bool>> RevokeRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdateRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    Task<Result<bool>> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default);

    Task<Result<bool>> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken = default);

    Task<Result<string>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}
