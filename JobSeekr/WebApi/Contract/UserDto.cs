namespace WebApi.Contract;

public record UserDto
{
    public long Id { get; init; }
    public required string Email { get; init; }
    public Role Role { get; init; }
}
