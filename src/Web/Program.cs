using ApiBaseTemplate.Application;
using ApiBaseTemplate.Infrastructure;
using ApiBaseTemplate.Infrastructure.Data;
using ApiBaseTemplate.Web;
using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;


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
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",  description.GroupName.ToUpperInvariant());
            }
        });
    }
    
    app.UseSerilogRequestLogging();
    app.UseHealthChecks("/health");
    
    app.UseHttpsRedirection();
    
    app.UseStaticFiles();
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.UseExceptionHandler(options => { });

    app.MapHealthChecks("/health",
        new HealthCheckOptions()
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
