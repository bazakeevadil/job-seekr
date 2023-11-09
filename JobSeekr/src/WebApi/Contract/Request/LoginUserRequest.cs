namespace WebApi.Contract.Request;

/// <summary>
/// Запрос для входа в систему
/// </summary>
public record LoginUserRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
