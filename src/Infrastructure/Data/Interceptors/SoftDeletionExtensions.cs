using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Infrastructure.Data.Interceptors.Interfaces;

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
}