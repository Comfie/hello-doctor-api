using System.Text;
using HelloDoctorApi.Application.Common.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HelloDoctorApi.Infrastructure.OptionsSetup
{
    public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AppSettings _appSettings;

        public JwtBearerOptionsSetup(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public void Configure(JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _appSettings.ValidIssuer,
                ValidAudience = _appSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)),
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };

            // Add event handlers for debugging
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine($"[JWT] Token validated successfully");
                    var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    Console.WriteLine($"[JWT] User ID from token: {userId}");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    Console.WriteLine($"[JWT] Authorization header: {authHeader?.Substring(0, Math.Min(50, authHeader?.Length ?? 0))}...");
                    return Task.CompletedTask;
                }
            };
        }

        public void Configure(string? name, JwtBearerOptions options) => Configure(options);
    }
}