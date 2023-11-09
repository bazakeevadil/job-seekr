namespace WebApi.Contract.Request;

/// <summary>
/// Запрос на получение пользователя
/// </summary>
public record GetAllUserRequest
{
    public long Id { get; init; }
    public required string Email { get; init; }
    public Role Role { get; init; }
}
