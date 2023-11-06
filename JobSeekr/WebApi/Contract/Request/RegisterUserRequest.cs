using System.ComponentModel.DataAnnotations;

namespace WebApi.Contract.Request;

public record RegisterUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
