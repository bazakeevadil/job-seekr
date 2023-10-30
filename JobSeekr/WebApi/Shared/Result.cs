namespace WebApi.Shared;

public abstract class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsError => IsSuccess != true;

    public List<Error> Errors { get; protected set; } = new();
    protected Result() { }

    public static Result<TValue> Ok<TValue>() => new();
    public static Result<TValue> Ok<TValue>(TValue value) => new(value);
    public static Result<TValue> Bad<TValue>(string massage) => new(new Error(massage));
    public static Result<TValue> Bad<TValue>(string massage, string code) => new(new Error(massage, code));
    public static Result<TValue> Bad<TValue>(Error error) => new(error);
    public static Result<TValue> Bad<TValue>(List<Error> errors) => new(errors);
}
