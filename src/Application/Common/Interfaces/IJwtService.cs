using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiBaseTemplate.Domain.Entities.Auth;
using Microsoft.IdentityModel.Tokens;

namespace ApiBaseTemplate.Application.Common.Interfaces;

public interface IJwtService
{
    SigningCredentials GetSigningCredentials();
    Task<List<Claim>> GetClaims(ApplicationUser user);
    JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
    string GenerateRefreshToken(ApplicationUser user);
}
