using WebApi.Enums;

namespace WebApi.Contract;

public record UserDto
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public Role Role { get; set; }
}
