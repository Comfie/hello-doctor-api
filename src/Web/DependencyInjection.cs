using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Infrastructure.Data;
using ApiBaseTemplate.Web.OptionsSetup;
using ApiBaseTemplate.Web.Services;
using Asp.Versioning;
using Grinderofl.FeatureFolders;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace ApiBaseTemplate.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
        
        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddHttpContextAccessor();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddTransient<IConfigureOptions<CorsOptions>, ConfigureCorsOptions>();

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection") ?? String.Empty)
            .AddDbContextCheck<ApplicationDbContext>();  //this one only necessary if you have multiple db context

        services.AddExceptionHandler<CustomExceptionHandler>();
        
        // services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);
        
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddEndpointsApiExplorer();
        services.AddControllers()
            .AddFeatureFolders()
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        
        // Register the Swagger generator, defining 1 or more Swagger documents
        services.AddSwaggerGen();
        services.AddCors();

        return services;
    }
    
}
