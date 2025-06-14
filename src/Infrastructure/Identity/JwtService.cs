using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Common.Models.Settings;
using HelloDoctorApi.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HelloDoctorApi.Infrastructure.Identity;

public class JwtService : IJwtService
{
    private readonly IDateTimeService _dateTime;
    private readonly AppSettings _appSettings;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(IDateTimeService dateTime, IOptions<AppSettings> appSettings,
        UserManager<ApplicationUser> userManager)
    {
        _dateTime = dateTime;
        _appSettings = appSettings.Value;
        _userManager = userManager;
    }

    public SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public async Task<List<Claim>> GetClaims(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            // Keep the original claims for compatibility
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            // Add simple claims
            new Claim("userId", user.Id),
            new Claim("email", user.UserName ?? string.Empty),
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role)); // Keep original
            claims.Add(new Claim("role", role)); // Add simple claim
        }

        return claims;
    }

    public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: _appSettings.ValidIssuer,
            audience: _appSettings.ValidAudience,
            claims: claims,
            expires: _dateTime.Now.AddHours(3),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }

    public Task<string> GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokeOptions = new JwtSecurityToken(
            issuer: _appSettings.ValidIssuer,
            audience: _appSettings.ValidAudience,
            claims: claims,
            expires: _dateTime.Now.AddHours(3),
            signingCredentials: GetSigningCredentials()
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return Task.FromResult(tokenString);
    }

    public string GenerateRefreshToken(ApplicationUser user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
        var secret = new SymmetricSecurityKey(key);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = secret,
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}