using Amazon.S3;
using Asp.Versioning;
using Grinderofl.FeatureFolders;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Infrastructure.Data;
using HelloDoctorApi.Web.OptionsSetup;
using HelloDoctorApi.Web.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HelloDoctorApi.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Add memory cache for performance optimization
        services.AddMemoryCache();

        services.Configure<S3FileStoreOptions>(
            configuration.GetSection(S3FileStoreOptions.ConfigurationSection));
        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IChecksumService, Sha256ChecksumService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddHttpClient<ISmsService, SmsService>();
        
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        
        if (configuration.GetValue<bool>("FileStorage:UseS3Storage"))
        {
            services.AddScoped<IFileStoreService, S3FileStoreService>();
        }
        else
        {
            services.AddScoped<IFileStoreService, LocalFileStoreService>();
        }

        services.AddHttpContextAccessor();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddTransient<IConfigureOptions<CorsOptions>, ConfigureCorsOptions>();

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection") ?? string.Empty)
            .AddDbContextCheck<ApplicationDbContext>(); //this one only necessary if you have multiple db context

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