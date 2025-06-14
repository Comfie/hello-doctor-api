using System.Linq.Expressions;
using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Infrastructure.Data.Interceptors.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Infrastructure.Data.Interceptors;

public static class SoftDeletionExtensions
{
    /// <summary>
    /// Exclude deleted items from a query
    /// </summary>
    /// <param name="queryable"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> NotDeleted<TQuery>(this IQueryable<TQuery> queryable) where TQuery : ISoftDelete
    {
        return queryable
            .Where(entity => !entity.IsDeleted);
    }

    /// <summary>
    /// Limit a query to only deleted items
    /// </summary>
    /// <param name="queryable"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> DeletedOnly<TQuery>(this IQueryable<TQuery> queryable) where TQuery : ISoftDelete
    {
        return queryable
            .Where(entity => entity.IsDeleted);
    }

    /// <summary>
    /// Filter by items deleted after a given date, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="time"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> DeletedAfter<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset time)
        where TQuery : ISoftDelete
    {
        return queryable
            .DeletedOnly()
            .Where(entity => entity.DeletedAt >= time);
    }

    /// <summary>
    /// Filter by items deleted before a given date, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="time"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> DeletedBefore<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset time)
        where TQuery : ISoftDelete
    {
        return queryable
            .DeletedOnly()
            .Where(entity => entity.DeletedAt <= time);
    }
    
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var lambdaExpression = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambdaExpression);
            }
        }
    }

    // Extension method to include soft-deleted entities when needed
    public static IQueryable<T> IncludeSoftDeleted<T>(this IQueryable<T> query) where T : class, ISoftDelete
    {
        return query.IgnoreQueryFilters();
    }

    // Extension method to explicitly exclude soft-deleted entities
    public static IQueryable<T> ExcludeSoftDeleted<T>(this IQueryable<T> query) where T : class, ISoftDelete
    {
        return query.Where(e => !e.IsDeleted);
    }

}