using Ardalis.Result;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace HelloDoctorApi.Application.Authentications.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeService _dateTime;

    public RefreshTokenHandler(IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService, IDateTimeService dateTime)
    {
        _context = context;
        _userManager = userManager;
        _jwtService = jwtService;
        _dateTime = dateTime;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
            return Result<AuthResponse>.Error(new Error("Invalid Token", "Invalid Token"));

        var username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return Result<AuthResponse>.Error(new Error("Invalid Token", "Invalid Token"));

        var claims = await _jwtService.GetClaims(user);
        var token = await _jwtService.GenerateAccessToken(claims);
        var userRoles = await _userManager.GetRolesAsync(user);
        var refreshToken = _jwtService.GenerateRefreshToken(user);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _dateTime.Now.AddDays(7);

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
}