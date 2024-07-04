using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Common.Models.Settings;
using ApiBaseTemplate.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiBaseTemplate.Infrastructure.Identity;

public class JwtService : IJwtService
{
    private readonly IDateTimeService _dateTime;
    private readonly AppSettings _appSettings;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(IDateTimeService dateTime, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
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
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
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
    
    public string GenerateRefreshToken(ApplicationUser user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
}
