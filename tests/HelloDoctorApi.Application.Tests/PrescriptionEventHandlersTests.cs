using FluentAssertions;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Prescriptions.Events;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Events;
using HelloDoctorApi.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HelloDoctorApi.Application.Tests;

public class FakeDbContext : DbContext, IApplicationDbContext
{
    public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options) {}
    public DbSet<HelloDoctorApi.Domain.Entities.Auth.ApplicationUser> ApplicationUsers => Set<HelloDoctorApi.Domain.Entities.Auth.ApplicationUser>();
    public DbSet<HelloDoctorApi.Domain.Entities.Auth.ApplicationRole> ApplicationRoles => Set<HelloDoctorApi.Domain.Entities.Auth.ApplicationRole>();
    public DbSet<HelloDoctorApi.Domain.Entities.Auth.SuperAdministrator> SuperAdministrators => Set<HelloDoctorApi.Domain.Entities.Auth.SuperAdministrator>();
    public DbSet<HelloDoctorApi.Domain.Entities.Auth.SystemAdministrator> SystemAdministrators => Set<HelloDoctorApi.Domain.Entities.Auth.SystemAdministrator>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
    public DbSet<MainMember> MainMembers => Set<MainMember>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Pharmacist> Pharmacists => Set<Pharmacist>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionNote> PrescriptionNotes => Set<PrescriptionNote>();
    public DbSet<PrescriptionStatusHistory> PrescriptionStatusHistories => Set<PrescriptionStatusHistory>();
    public DbSet<FileUpload> FileUploads => Set<FileUpload>();
    public DbSet<HelloDoctorApi.Domain.Entities.Auth.OneTimePin> OneTimePins => Set<HelloDoctorApi.Domain.Entities.Auth.OneTimePin>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
}

public class FakeUser : IUser
{
    public string? Id { get; set; } = "tester";
    public long? GetPharmacyId() => null;
    public long? GetMainMemberId() => null;
    public long? GetDoctorId() => null;
}
public class FakeClock : IDateTimeService { public DateTime Now => DateTime.UtcNow; public DateTimeOffset OffsetNow => DateTimeOffset.UtcNow; }

public class PrescriptionEventHandlersTests
{
    private ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        services.AddDbContext<FakeDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<FakeDbContext>());
        services.AddScoped<IUser, FakeUser>();
        services.AddScoped<IDateTimeService, FakeClock>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PrescriptionAssignedHandler).Assembly));
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task AssignedEvent_WritesHistory_And_Audit()
    {
        var sp = BuildServices();
        var db = sp.GetRequiredService<FakeDbContext>();
        var mediator = sp.GetRequiredService<IMediator>();

        var user = new HelloDoctorApi.Domain.Entities.Auth.ApplicationUser { Id = "user" };
        var pres = new Prescription
        {
            IssuedDate = DateTimeOffset.UtcNow,
            ExpiryDate = DateTimeOffset.UtcNow.AddDays(7),
            Status = PrescriptionStatus.Pending,
            MainMemberId = "user",
            MainMember = user,
            BeneficiaryId = 1,
            Beneficiary = new Beneficiary
            {
                FirstName = "John", LastName = "Doe", PhoneNumber = "123",
                EmailAddress = "j@d.com", BeneficiaryCode = "B1",
                Relationship = RelationshipToMainMember.Child,
                MainMemberId = "user",
                MainMember = user,
                Gender = "male",
                DateOfBirth = DateTime.Now.AddYears(-18)
            }
        };
        db.Prescriptions.Add(pres);
        await db.SaveChangesAsync();

        await mediator.Publish(new PrescriptionAssignedToPharmacyEvent(pres.Id, 9, PrescriptionStatus.Pending, PrescriptionStatus.UnderReview));

        (await db.PrescriptionStatusHistories.CountAsync()).Should().Be(1);
        (await db.AuditLogs.CountAsync()).Should().Be(1);
    }
}

