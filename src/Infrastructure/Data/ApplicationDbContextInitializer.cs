using ApiBaseTemplate.Domain.Entities.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiBaseTemplate.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitialiseAsync();

        await initializer.SeedAsync();
    }
}

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        try
        {
            if (!_context.Database.IsNpgsql())
            {
                _logger.LogError("Migration Failed, the DbContext was not for Npgsql");
                return;
            }

            if (!await _context.Database.CanConnectAsync(cancellationTokenSource.Token))
            {
                _logger.LogInformation("Unable to connect to the specified database, it will be created");
            }

            try
            {
                var pendingMigrations = await _context.Database
                    .GetPendingMigrationsAsync(cancellationTokenSource.Token);
                IEnumerable<string> migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
                var pendingMigrationCount = migrations.Count();
                if (pendingMigrationCount == 0)
                {
                    _logger.LogInformation("No pending migrations found. Database is up to date");
                    return;
                }

                var pending = migrations
                    .Aggregate((first, next) => first + ", " + next);
                _logger.LogInformation("Pending Migrations: {Migrations}", pending);
                _logger.LogInformation("Applying {Count} Migrations", pendingMigrationCount);
                await _context.Database.MigrateAsync(cancellationTokenSource.Token);

                if (_context.Database.IsNpgsql())
                {
                    await _context.Database.MigrateAsync(cancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting pending migrations");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
        }

        cancellationTokenSource.Dispose();
    }

    public async Task SeedAsync()
    {
        try
        {
            await SetupRoles();
            await SeedFirstSuperAdmin();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SetupRoles()
    {
        try
        {
            var roles = new List<string>();
            roles.Add("Administrator");
            roles.Add("User");

            foreach (var role in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Role {Role} created", role);
                }

                _logger.LogInformation("Role {Role} exists", role);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    public async Task SeedFirstSuperAdmin()
    {
        var user = await _userManager.FindByEmailAsync("comfynyatsine@gmail.com");

        if (user is not null)
        {
            _logger.LogInformation("ApplicationUser {ApplicationUser} already exists", user.Email);
            return;
        }

        // Default users
        var administrator = new ApplicationUser
        {
            UserName = "comfynyatsine@gmail.com",
            Email = "comfynyatsine@gmail.com",
            EmailConfirmed = true,
            FirstName = "Comfort",
            LastName = "Nyatsine",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            var administratorRole = new IdentityRole("Administrator");

            await _userManager.CreateAsync(administrator, "Admin@123");
            _logger.LogInformation("ApplicationUser {ApplicationUser} created", administrator.Email);
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }
    }
}
