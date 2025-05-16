namespace HelloDoctorApi.Domain.Shared;

public class ResultLocal<TValue> : ResultLocal
{
    private readonly TValue? _value;

    protected internal ResultLocal(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure resultLocal can not be accessed.");

    public static implicit operator ResultLocal<TValue>(TValue? value) => Create(value);
}