using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Entities.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HelloDoctorApi.Infrastructure.Data;

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
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
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
            await SeedFirstSystemAdmin();
            await SeedFirstPharmacy();
            await SeedFirstMainMember();
            await SeedFirstDoctor();
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
            string[] roles =
            [
                "SystemAdministrator",
                "SuperAdministrator",
                "Doctor",
                "Beneficiary",
                "Pharmacist",
                "MainMember",
                "LogisticsPartner",
                "User"
            ];

            foreach (var role in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = role });
                    _logger.LogInformation("Role {Role} created", role);
                    continue;
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
        var user = await _userManager.FindByEmailAsync("superadmin@gmail.com");

        if (user is not null)
        {
            _logger.LogInformation("ApplicationUser {ApplicationUser} already exists", user.Email);
            return;
        }

        var superAdmin = new ApplicationUser
        {
            UserName = "superadmin@gmail.com",
            Email = "superadmin@gmail.com",
            EmailConfirmed = true,
            FirstName = "Super",
            LastName = "Admin",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        if (_userManager.Users.All(u => u.UserName != superAdmin.UserName))
        {
            await _userManager.CreateAsync(superAdmin, "Admin@123");
            _logger.LogInformation("ApplicationUser {ApplicationUser} created", superAdmin.Email);

            await _userManager.AddToRoleAsync(superAdmin, "SuperAdministrator");

            // Create SuperAdministrator profile
            var superAdminProfile = new SuperAdministrator
            {
                UserId = superAdmin.Id,
                User = superAdmin
            };

            _context.SuperAdministrators.Add(superAdminProfile);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedFirstSystemAdmin()
    {
        var user = await _userManager.FindByEmailAsync("systemadmin@gmail.com");

        if (user is not null)
        {
            _logger.LogInformation("ApplicationUser {ApplicationUser} already exists", user.Email);
            return;
        }

        var systemAdmin = new ApplicationUser
        {
            UserName = "systemadmin@gmail.com",
            Email = "systemadmin@gmail.com",
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        if (_userManager.Users.All(u => u.UserName != systemAdmin.UserName))
        {
            await _userManager.CreateAsync(systemAdmin, "Admin@123");
            _logger.LogInformation("ApplicationUser {ApplicationUser} created", systemAdmin.Email);

            await _userManager.AddToRoleAsync(systemAdmin, "SystemAdministrator");

            // Create SystemAdministrator profile
            var systemAdminProfile = new SystemAdministrator
            {
                UserId = systemAdmin.Id,
                User = systemAdmin
            };

            _context.SystemAdministrators.Add(systemAdminProfile);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedFirstMainMember()
    {
        var user = await _userManager.FindByEmailAsync("member@gmail.com");

        if (user is not null)
        {
            _logger.LogInformation("ApplicationUser {ApplicationUser} already exists", user.Email);
            return;
        }

        var mainMemberUser = new ApplicationUser
        {
            UserName = "member@gmail.com",
            Email = "member@gmail.com",
            EmailConfirmed = true,
            FirstName = "Main",
            LastName = "Member",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        if (_userManager.Users.All(u => u.UserName != mainMemberUser.UserName))
        {
            await _userManager.CreateAsync(mainMemberUser, "Admin@123");
            _logger.LogInformation("ApplicationUser {ApplicationUser} created", mainMemberUser.Email);

            await _userManager.AddToRoleAsync(mainMemberUser, "MainMember");

            // Create MainMember profile
            var mainMemberProfile = new MainMember
            {
                Code = "MM001",
                AccountId = mainMemberUser.Id,
                Account = mainMemberUser,
                DefaultPharmacyId = null
            };

            _context.MainMembers.Add(mainMemberProfile);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedFirstPharmacy()
    {
        var pharmacy = _context.Pharmacies.FirstOrDefault();

        if (pharmacy is not null)
        {
            _logger.LogInformation("Pharmacy {Pharmacy} already exists", pharmacy.Name);
            // Still try to seed pharmacist for this pharmacy
            await SeedFirstPharmacist(pharmacy.Id);
            return;
        }

        pharmacy = new Pharmacy
        {
            Name = "Test Pharmacy",
            Description = "Test pharmacy for development",
            Address = "123 Main Street, Test City",
            ContactEmail = "pharmacy@testpharmacy.com",
            ContactPerson = "Pharmacy Manager",
            ContactNumber = "+1234567890",
            IsActive = true,
            OpeningTime = TimeSpan.FromHours(8),
            ClosingTime = TimeSpan.FromHours(17),
        };

        _context.Pharmacies.Add(pharmacy);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Pharmacy {Pharmacy} created", pharmacy.Name);

        // Seed pharmacist for this pharmacy
        await SeedFirstPharmacist(pharmacy.Id);
    }

    public async Task SeedFirstPharmacist(long pharmacyId)
    {
        var user = await _userManager.FindByEmailAsync("pharmacist@gmail.com");

        // Create user if it doesn't exist
        if (user is null)
        {
            var pharmacistUser = new ApplicationUser
            {
                UserName = "pharmacist@gmail.com",
                Email = "pharmacist@gmail.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Pharmacist",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _userManager.CreateAsync(pharmacistUser, "Admin@123");
            _logger.LogInformation("ApplicationUser {ApplicationUser} created", pharmacistUser.Email);

            await _userManager.AddToRoleAsync(pharmacistUser, "Pharmacist");
            user = pharmacistUser;
        }
        else
        {
            _logger.LogInformation("ApplicationUser {ApplicationUser} already exists", user.Email);

            // Ensure the Pharmacist role is assigned even if user already exists
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Pharmacist"))
            {
                _logger.LogInformation("Adding Pharmacist role to existing user {Email}", user.Email);
                await _userManager.AddToRoleAsync(user, "Pharmacist");
            }
        }

        // Check if Pharmacist profile exists
        _logger.LogInformation("Checking for Pharmacist profile for user {UserId} ({Email})", user.Id, user.Email);
        var pharmacistProfile = await _context.Pharmacists
            .FirstOrDefaultAsync(p => p.AccountId == user.Id);

        if (pharmacistProfile is null)
        {
            _logger.LogInformation(
                "Pharmacist profile not found. Creating new profile for {Email} linked to Pharmacy {PharmacyId}",
                user.Email, pharmacyId);

            // Create Pharmacist profile linking to pharmacy
            pharmacistProfile = new Pharmacist
            {
                AccountId = user.Id,
                Account = user,
                PharmacyId = pharmacyId,
                Pharmacy = null!
            };

            _context.Pharmacists.Add(pharmacistProfile);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedFirstDoctor()
    {
        var user = await _userManager.FindByEmailAsync("doctor@gmail.com");

        if (user is not null)
        {
            _logger.LogInformation("ApplicationUser {ApplicationUser} already exists", user.Email);
            return;
        }

        var doctorUser = new ApplicationUser
        {
            UserName = "doctor@gmail.com",
            Email = "doctor@gmail.com",
            EmailConfirmed = true,
            FirstName = "Dr. Jane",
            LastName = "Smith",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        if (_userManager.Users.All(u => u.UserName != doctorUser.UserName))
        {
            await _userManager.CreateAsync(doctorUser, "Admin@123");
            _logger.LogInformation("ApplicationUser {ApplicationUser} created", doctorUser.Email);

            await _userManager.AddToRoleAsync(doctorUser, "Doctor");
        }
        var doctorCheck = await _context.Doctors.FirstOrDefaultAsync(d => d.EmailAddress == doctorUser.Email);
        if (doctorCheck is null)
        {
            _logger.LogInformation("Doctor not found, creating doctor");
            var doctor = new Doctor
            {
                AccountId = doctorUser.Id,
                Account = doctorUser,
                FirstName = doctorUser.FirstName,
                LastName = doctorUser.LastName,
                EmailAddress = doctorUser.LastName,
                PrimaryContact = doctorUser.PhoneNumber ?? string.Empty,
                QualificationDescription = "Degree in Human Resources"
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }
    }
}