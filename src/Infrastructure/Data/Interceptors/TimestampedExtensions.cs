using HelloDoctorApi.Infrastructure.Data.Interceptors.Interfaces;

namespace HelloDoctorApi.Infrastructure.Data.Interceptors;

public static class TimestampedExtensions
{
    /// <summary>
    /// Filter by items created after a given date, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="time"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> CreatedAfter<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset time)
        where TQuery : ITimestamped
    {
        return queryable
            .Where(entity => entity.CreatedAt >= time);
    }

    /// <summary>
    /// Filter by items created before a given date, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="time"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> CreatedBefore<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset time)
        where TQuery : ITimestamped
    {
        return queryable
            .Where(entity => entity.CreatedAt <= time);
    }

    /// <summary>
    /// Filter by items updated after a given date, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="time"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> UpdatedAfter<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset time)
        where TQuery : ITimestamped
    {
        return queryable
            .Where(entity => entity.UpdatedAt >= time);
    }

    /// <summary>
    /// Filter by items updated before a given date, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="time"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> UpdatedBefore<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset time)
        where TQuery : ITimestamped
    {
        return queryable
            .Where(entity => entity.UpdatedAt <= time);
    }

    /// <summary>
    /// Filter by items created after <paramref name="start"/> and before <paramref name="end"/>, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> CreatedBetween<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset start,
        DateTimeOffset end) where TQuery : ITimestamped
    {
        return queryable
            .Where(entity => entity.CreatedAt >= start && entity.CreatedAt <= end);
    }

    /// <summary>
    /// Filter by items updated after <paramref name="start"/> and before <paramref name="end"/>, inclusive.
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <returns></returns>
    public static IQueryable<TQuery> UpdatedBetween<TQuery>(this IQueryable<TQuery> queryable, DateTimeOffset start,
        DateTimeOffset end) where TQuery : ITimestamped
    {
        return queryable
            .Where(entity => entity.UpdatedAt >= start && entity.UpdatedAt <= end);
    }
}