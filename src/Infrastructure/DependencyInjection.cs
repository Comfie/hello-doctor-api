using System.Security.Claims;
using System.Text;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Common.Models.Settings;
using HelloDoctorApi.Domain.Constants;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Infrastructure.Data;
using HelloDoctorApi.Infrastructure.Data.Interceptors;
using HelloDoctorApi.Infrastructure.Identity;
using HelloDoctorApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace HelloDoctorApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.Configure<AppSettings>(configuration.GetSection(AppSettings.Section));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.Section));
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(dataSource,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    builder.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    builder.CommandTimeout(10);
                });
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Set to true in production

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["AppSettings:ValidIssuer"],
                    ValidAudience = configuration["AppSettings:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["AppSettings:Secret"] ?? string.Empty)),
                    ClockSkew = TimeSpan.Zero,
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        Console.WriteLine($"[JWT Validated] Claims count: {claimsIdentity?.Claims.Count()}");

                        foreach (var claim in claimsIdentity?.Claims ?? Enumerable.Empty<Claim>())
                        {
                            Console.WriteLine($"[JWT Validated] Claim: {claim.Type} = {claim.Value}");
                        }

                        // Check specifically for pharmacyId
                        var pharmacyIdClaim = context.Principal?.FindFirst("pharmacyId");
                        Console.WriteLine($"[JWT Validated] pharmacyId claim found: {pharmacyIdClaim?.Value}");

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT Failed] Authentication failed: {context.Exception}");
                        return Task.CompletedTask;
                    }
                };
            });


        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
            options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        });

        services.AddAuthorizationBuilder();

        services
            .AddIdentity<ApplicationUser, ApplicationRole>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IMainMemberService, MainMainMemberService>();
        services.AddScoped<INotificationService, Services.Notifications.EmailNotificationService>();

        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        return services;
    }
}