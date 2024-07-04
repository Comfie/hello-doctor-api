using ApiBaseTemplate.Domain.Common;
using ApiBaseTemplate.Domain.Repositories;
using ApiBaseTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiBaseTemplate.Infrastructure;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        return _dbContext.SaveChangesAsync(cancellationToken);
    }
    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<BaseAuditableEntity>> entries =
            _dbContext
                .ChangeTracker
                .Entries<BaseAuditableEntity>();

        foreach (EntityEntry<BaseAuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.Created)
                    .CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.LastModified)
                    .CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
