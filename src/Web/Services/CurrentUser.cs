using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private JwtSecurityToken? _decodedToken;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // First try to get from ClaimsPrincipal
            var userId = user?.FindFirstValue("userId")
                         ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);

            // If not found and user is authenticated, parse JWT
            if (string.IsNullOrEmpty(userId) && user?.Identity?.IsAuthenticated == true)
            {
                var token = GetDecodedToken();
                userId = token?.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value;
            }

            Console.WriteLine($"[CurrentUser] User ID: {userId}");
            return userId;
        }
    }

    public long? GetPharmacyId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        // Try from ClaimsPrincipal first
        var pharmacyIdClaim = user?.FindFirst("pharmacyId")?.Value;

        // If not found, get from JWT token directly
        if (string.IsNullOrEmpty(pharmacyIdClaim))
        {
            var token = GetDecodedToken();
            pharmacyIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "pharmacyId")?.Value;
            Console.WriteLine($"[GetPharmacyId] Found in JWT: {pharmacyIdClaim}");
        }

        return long.TryParse(pharmacyIdClaim, out var pharmacyId) ? pharmacyId : null;
    }

    public long? GetMainMemberId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var mainMemberIdClaim = user?.FindFirst("mainMemberId")?.Value;

        if (string.IsNullOrEmpty(mainMemberIdClaim))
        {
            var token = GetDecodedToken();
            mainMemberIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "mainMemberId")?.Value;
        }

        return long.TryParse(mainMemberIdClaim, out var mainMemberId) ? mainMemberId : null;
    }

    public long? GetDoctorId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var doctorIdClaim = user?.FindFirst("doctorId")?.Value;

        if (string.IsNullOrEmpty(doctorIdClaim))
        {
            var token = GetDecodedToken();
            doctorIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "doctorId")?.Value;
        }

        return long.TryParse(doctorIdClaim, out var doctorId) ? doctorId : null;
    }

    private JwtSecurityToken? GetDecodedToken()
    {
        if (_decodedToken != null)
            return _decodedToken;

        try
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    _decodedToken = handler.ReadJwtToken(token);

                    Console.WriteLine($"[GetDecodedToken] JWT Claims count: {_decodedToken.Claims.Count()}");
                    foreach (var claim in _decodedToken.Claims)
                    {
                        Console.WriteLine($"[GetDecodedToken] Claim: {claim.Type} = {claim.Value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetDecodedToken] Error: {ex.Message}");
        }

        return _decodedToken;
    }
}