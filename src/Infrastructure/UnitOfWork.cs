using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HelloDoctorApi.Infrastructure;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDateTimeService _dateTime;

    public UnitOfWork(ApplicationDbContext dbContext, IDateTimeService dateTime)
    {
        _dbContext = dbContext;
        _dateTime = dateTime;
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
                    .CurrentValue = _dateTime.OffsetNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.LastModified)
                    .CurrentValue = _dateTime.OffsetNow;
            }
        }
    }
}