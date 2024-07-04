using System.Text;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Common.Models.Settings;
using ApiBaseTemplate.Domain.Constants;
using ApiBaseTemplate.Domain.Entities.Auth;
using ApiBaseTemplate.Infrastructure.Data;
using ApiBaseTemplate.Infrastructure.Data.Interceptors;
using ApiBaseTemplate.Infrastructure.Identity;
using ApiBaseTemplate.Infrastructure.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ApiBaseTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.Configure<AppSettings>(configuration.GetSection(AppSettings.Section));
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthorizationBuilder();

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IJwtService, JwtService>();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        return services;
    }
}
