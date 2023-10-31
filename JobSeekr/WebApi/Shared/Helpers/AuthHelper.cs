using WebApi.Domain.Entities;

namespace WebApi.Shared.Helpers;

public static class AuthHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool CheckPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.HashPassword);
    }
}
