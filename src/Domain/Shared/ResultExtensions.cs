namespace HelloDoctorApi.Domain.Shared;

public static class ResultExtensions
{
    public static ResultLocal<T> Ensure<T>(
        this ResultLocal<T> resultLocal,
        Func<T, bool> predicate,
        Error error)
    {
        if (resultLocal.IsFailure)
        {
            return resultLocal;
        }

        return predicate(resultLocal.Value) ? resultLocal : ResultLocal.Failure<T>(error);
    }

    public static ResultLocal<TOut> Map<TIn, TOut>(
        this ResultLocal<TIn> resultLocal,
        Func<TIn, TOut> mappingFunc)
    {
        return resultLocal.IsSuccess
            ? ResultLocal.Success(mappingFunc(resultLocal.Value))
            : ResultLocal.Failure<TOut>(resultLocal.Error);
    }
}