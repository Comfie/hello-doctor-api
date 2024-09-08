using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HelloDoctorApi.Domain.Entities.Auth;
using Microsoft.IdentityModel.Tokens;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IJwtService
{
    SigningCredentials GetSigningCredentials();
    Task<List<Claim>> GetClaims(ApplicationUser user);
    JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
    string GenerateRefreshToken(ApplicationUser user);
}
