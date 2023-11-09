namespace WebApi.Shared;

public abstract class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;

    public List<Error> Errors { get; protected set; } = new();
    protected Result() { }

    public static Result Ok() => new Result<object>();
    public static Result<TValue> Ok<TValue>() => new();
    public static Result<TValue> Ok<TValue>(TValue value) => new(value);

    public static Result Fail(string message) => new Result<object>(new Error(message));
    public static Result Fail(List<Error> errors) => new Result<object>(errors);
    public static Result<TValue> Fail<TValue>(string message) => new(new Error(message));
    public static Result<TValue> Fail<TValue>(string message, string code) => new(new Error(message, code));
    public static Result<TValue> Fail<TValue>(Error error) => new(error);
    public static Result<TValue> Fail<TValue>(List<Error> errors) => new(errors);
}
