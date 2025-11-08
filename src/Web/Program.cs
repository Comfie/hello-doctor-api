using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using HelloDoctorApi.Application;
using HelloDoctorApi.Infrastructure;
using HelloDoctorApi.Infrastructure.Data;
using HelloDoctorApi.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

using System.IdentityModel.Tokens.Jwt;

// Clear the default claim type mapping to preserve custom claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    Log.Information("starting server.");

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.MinimumLevel.Warning();
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebServices(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp",
            policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });

    var app = builder.Build();

    // Initialise and seed database
    await app.InitialiseDatabaseAsync();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
        });
    }
    
    app.UseHttpsRedirection();
    app.UseCors("AllowAngularApp");
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.UseSerilogRequestLogging();
    app.UseHealthChecks("/health");

    app.UseStaticFiles();

    app.UseExceptionHandler(options => { });

    app.MapHealthChecks("/health",
        new HealthCheckOptions
        {
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status418ImATeapot,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            },
            AllowCachingResponses = false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}