using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<ApplicationRole> ApplicationRoles { get; }
    DbSet<SuperAdministrator> SuperAdministrators { get; }
    DbSet<SystemAdministrator> SystemAdministrators { get; }
    DbSet<OneTimePin> OneTimePins { get; }
    DbSet<Beneficiary> Beneficiaries { get; }
    DbSet<Pharmacy> Pharmacies { get; }
    DbSet<MainMember> MainMembers { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Pharmacist> Pharmacists { get; }
    DbSet<Prescription> Prescriptions { get; }
    DbSet<PrescriptionNote> PrescriptionNotes { get; }
    DbSet<PrescriptionStatusHistory> PrescriptionStatusHistories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}