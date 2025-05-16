using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Infrastructure.Data.Interceptors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HelloDoctorApi.Infrastructure.Data.Interceptors;

public sealed class TimestampedInterceptor(IDateTimeService dateTime) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(
                eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<ITimestamped>> entries =
            eventData
                .Context
                .ChangeTracker
                .Entries<ITimestamped>();

        foreach (EntityEntry<ITimestamped> entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                {
                    entry.Entity.CreatedAt = dateTime.OffsetNow;
                    entry.Entity.UpdatedAt = dateTime.OffsetNow;
                    continue;
                }
                case EntityState.Modified:
                {
                    entry.Entity.UpdatedAt = dateTime.OffsetNow;
                    continue;
                }
                case EntityState.Deleted:
                case EntityState.Detached:
                case EntityState.Unchanged:
                    continue;
                default:
                    throw new NotImplementedException($"Unhandled EntityState: {entry.State}");
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}