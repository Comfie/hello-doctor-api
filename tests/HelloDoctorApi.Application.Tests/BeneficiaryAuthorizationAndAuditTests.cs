using FluentAssertions;
using HelloDoctorApi.Application.Beneficiaries.Commands.CreateBeneficiary;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Beneficiaries.Updates.DeleteBeneficiary;
using HelloDoctorApi.Application.Beneficiaries.Updates.UpdateBeneficiary;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HelloDoctorApi.Application.Tests;

public class BeneficiaryAuthorizationAndAuditTests
{
    private class FakeDbContext : DbContext, IApplicationDbContext
    {
        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options) { }
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

    private class FakeUnitOfWork : HelloDoctorApi.Domain.Repositories.IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private class FakeUser : IUser
    {
        public string? Id { get; set; } = "member-1";
        public long? GetPharmacyId() => null;
        public long? GetMainMemberId() => null;
        public long? GetDoctorId() => null;
    }

    private class SwitchableUser : IUser
    {
        public string? Id { get; set; }
        public long? GetPharmacyId() => null;
        public long? GetMainMemberId() => null;
        public long? GetDoctorId() => null;
    }

    private ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        services.AddDbContext<FakeDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<FakeDbContext>());
        services.AddScoped<IUser, SwitchableUser>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBeneficiaryCommand).Assembly));
        services.AddScoped<HelloDoctorApi.Domain.Repositories.IUnitOfWork, FakeUnitOfWork>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task CreateBeneficiary_WritesAudit_ForOwner()
    {
        var sp = BuildServices();
        var db = sp.GetRequiredService<FakeDbContext>();

        var owner = new HelloDoctorApi.Domain.Entities.Auth.ApplicationUser { Id = "member-1", FirstName = "Ann", LastName = "O" };
        db.ApplicationUsers.Add(owner);
        await db.SaveChangesAsync();

        var cmd = new CreateBeneficiaryCommand(new CreateBeneficiaryRequest(
            MainMemberId: owner.Id,
            LastName: "Doe",
            PhoneNumber: "123",
            FirstName: "John",
            EmailAddress: "j@d.com",
            Relationship: "child",
            Gender: "male",
            DateOfBirth: DateTime.Now.AddYears(-18)
        ));

        var currentUser = (SwitchableUser)sp.GetRequiredService<IUser>();
        currentUser.Id = owner.Id;

        var mediator = sp.GetRequiredService<MediatR.IMediator>();
        var result = await mediator.Send(cmd);
        result.IsSuccess.Should().BeTrue();

        (await db.AuditLogs.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task UpdateBeneficiary_Fails_ForNonOwner()
    {
        var sp = BuildServices();
        var db = sp.GetRequiredService<FakeDbContext>();

        var owner = new HelloDoctorApi.Domain.Entities.Auth.ApplicationUser { Id = "owner", FirstName = "Ann", LastName = "O" };
        var other = new HelloDoctorApi.Domain.Entities.Auth.ApplicationUser { Id = "other", FirstName = "Bob", LastName = "X" };
        db.ApplicationUsers.AddRange(owner, other);
        db.Beneficiaries.Add(new Beneficiary
        {
            FirstName = "Ben", LastName = "Ef", PhoneNumber = "123",
            EmailAddress = "b@e.com", BeneficiaryCode = "B1",
            Relationship = RelationshipToMainMember.Child,
            MainMemberId = owner.Id, MainMember = owner,
            Gender = "male",
            DateOfBirth = DateTime.Now.AddYears(-18)
        });
        await db.SaveChangesAsync();

        // act as other user
        var currentUser = (SwitchableUser)sp.GetRequiredService<IUser>();
        currentUser.Id = other.Id;

        var mediator = sp.GetRequiredService<MediatR.IMediator>();
        var update = new UpdateBeneficiaryCommand(1, FirstName: "New", LastName: null, PhoneNumber: null, EmailAddress: null,
            Gender: null, DateOfBirth: null, RelationshipToMainMember: "child");
        var result = await mediator.Send(update);

        result.Status.Should().Be(Ardalis.Result.ResultStatus.Forbidden);
    }

    [Fact]
    public async Task DeleteBeneficiary_WritesAudit_ForOwner()
    {
        var sp = BuildServices();
        var db = sp.GetRequiredService<FakeDbContext>();
        var owner = new HelloDoctorApi.Domain.Entities.Auth.ApplicationUser { Id = "member-1", FirstName = "Ann", LastName = "O" };
        db.ApplicationUsers.Add(owner);

        var currentUser = (SwitchableUser)sp.GetRequiredService<IUser>();
        currentUser.Id = owner.Id;
        db.Beneficiaries.Add(new Beneficiary
        {
            FirstName = "Ben", LastName = "Ef", PhoneNumber = "123",
            EmailAddress = "b@e.com", BeneficiaryCode = "B1",
            Relationship = RelationshipToMainMember.Child,
            MainMemberId = owner.Id, MainMember = owner,
            Gender = "male",
            DateOfBirth = DateTime.Now.AddYears(-18)
        });
        await db.SaveChangesAsync();

        var mediator = sp.GetRequiredService<MediatR.IMediator>();
        var result = await mediator.Send(new DeleteBeneficiaryCommand(1));
        result.IsSuccess.Should().BeTrue();

        (await db.AuditLogs.CountAsync()).Should().Be(1);
    }
}

