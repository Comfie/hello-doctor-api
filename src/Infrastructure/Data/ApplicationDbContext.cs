using System.Linq.Expressions;
using System.Reflection;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
    public DbSet<SuperAdministrator> SuperAdministrators => Set<SuperAdministrator>();
    public DbSet<SystemAdministrator> SystemAdministrators => Set<SystemAdministrator>();
    public DbSet<OneTimePin> OneTimePins => Set<OneTimePin>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
    public DbSet<MainMember> MainMembers => Set<MainMember>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Pharmacist> Pharmacists => Set<Pharmacist>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionNote> PrescriptionNotes => Set<PrescriptionNote>();
    public DbSet<PrescriptionStatusHistory> PrescriptionStatusHistories => Set<PrescriptionStatusHistory>();
    public DbSet<FileUpload> FileUploads => Set<FileUpload>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

   
    }
}