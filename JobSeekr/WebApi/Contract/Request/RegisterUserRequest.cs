namespace WebApi.Contract.Request;

/// <summary>
/// Запрос для регистрации.
/// </summary>
public record RegisterUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
