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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Apply soft delete filter to other entities / remove for now
        // foreach (var entityType in builder.Model.GetEntityTypes())
        // {
        //     // Skip IdentityRole and its derived types when applying the filter
        //     if (typeof(IdentityRole).IsAssignableFrom(entityType.ClrType))
        //         continue;
        //
        //     if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
        //     {
        //         var parameter = Expression.Parameter(entityType.ClrType, "e");
        //         var property = Expression.PropertyOrField(parameter, nameof(ISoftDelete.IsDeleted));
        //         var falseConstant = Expression.Constant(false);
        //         var lambdaExpression = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);
        //
        //         builder.Entity(entityType.ClrType).HasQueryFilter(lambdaExpression);
        //     }
        // }


    }
}