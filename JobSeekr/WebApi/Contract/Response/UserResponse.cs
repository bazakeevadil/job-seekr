namespace WebApi.Contract.Response;

public record UserResponse
{
    public long Id { get; init; }
    public required string Email { get; init; }
    public required bool IsBlocked { get; init; }
    public Role Role { get; init; }
}
