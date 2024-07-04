using ApiBaseTemplate.Domain.Entities.Auth;

namespace ApiBaseTemplate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<OneTimePin> OneTimePins { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
