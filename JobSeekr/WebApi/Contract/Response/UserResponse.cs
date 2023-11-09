namespace WebApi.Contract.Response;

/// <summary>
/// Ответ для получения пользователя.
/// </summary>
public record UserResponse
{
    public long Id { get; init; }
    public required string Email { get; init; }
    public required bool IsBlocked { get; init; }
    public Role Role { get; init; }
}
