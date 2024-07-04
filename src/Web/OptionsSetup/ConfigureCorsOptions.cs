using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace ApiBaseTemplate.Web.OptionsSetup;

public class ConfigureCorsOptions : IConfigureOptions<CorsOptions>
{
    public void Configure(CorsOptions options)
    {
        options.AddPolicy("CorsPolicy", builder => builder
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials()
       .WithOrigins("http://localhost:4200/",
           "https://localhost:4200/",
           "https://localhost:8080",
           "http://localhost:8080",
           "http://localhost:5000",
           "https://localhost:5001")
       .AllowAnyHeader()
       .AllowAnyMethod());
    }
}
