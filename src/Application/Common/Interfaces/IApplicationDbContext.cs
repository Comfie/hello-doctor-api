using ApiBaseTemplate.Domain.Entities;
using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<SuperAdministrator> SuperAdministrators { get; }
    DbSet<SystemAdministrator> SystemAdministrators { get; }
    DbSet<OneTimePin> OneTimePins { get; }
    DbSet<Beneficiary> Beneficiaries { get; }
    DbSet<Pharmacy> Pharmacies { get; }
    DbSet<MainMember> MainMembers { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Pharmacist> Pharmacists { get; }
    DbSet<Prescription> Prescriptions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
