namespace WebApi.Contract.Request;

public record LoginUserRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
