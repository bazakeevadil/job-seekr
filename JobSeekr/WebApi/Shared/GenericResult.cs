namespace WebApi.Shared;

public class Result<TValue> : Result
{
    public TValue? Value { get; private set; }
    public bool HasValue { get; private set; }

    public Result()
    {
        IsSuccess = true;
        Value = default;
        HasValue = false;
    }

    public Result(TValue value)
    {
        IsSuccess = true;
        Value = value;
        HasValue = false;
    }

    public Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        HasValue = false;
        Errors.Add(error);
    }

    public Result(List<Error> errors)
    {
        IsSuccess = false;
        Value = default;
        HasValue = false;
        Errors = errors;
    }

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

    public void Deconstruct(out bool isSuccess, out TValue? value, out List<Error> errors)
    {
        isSuccess = IsSuccess;
        value = Value;
        errors = Errors;
    }
}
