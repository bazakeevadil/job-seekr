namespace WebApi.Contract.Request;

public record GetAllUserRequest
{
    public long Id { get; init; }
    public required string Email { get; init; }
    public Role Role { get; init; }
}
