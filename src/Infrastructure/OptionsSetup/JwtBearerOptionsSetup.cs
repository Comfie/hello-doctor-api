using System.Text;
using ApiBaseTemplate.Application.Common.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiBaseTemplate.Infrastructure.OptionsSetup
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
                ClockSkew = TimeSpan.Zero
            };
        }

        public void Configure(string? name, JwtBearerOptions options) => Configure(options);
    }
}
