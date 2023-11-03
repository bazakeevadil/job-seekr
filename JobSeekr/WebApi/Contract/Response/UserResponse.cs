namespace WebApi.Contract.Response;

public record UserResponse
{
    public long Id { get; init; }
    public required string Email { get; init; }
    public Role Role { get; init; }
}
