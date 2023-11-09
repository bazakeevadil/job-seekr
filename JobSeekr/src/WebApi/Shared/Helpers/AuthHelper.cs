namespace WebApi.Shared.Helpers;

/// <summary>
/// Вспомогательный класс для аутентификации.
/// </summary>
public static class AuthHelper
{
    /// <summary>
    ///  Хэширует пароль с помощью функции хеширования BCrypt.
    /// </summary>
    /// <param name="password">Пароль, который требуется захешировать.</param>
    /// <returns>Хэш-строка пароля.</returns>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Проверяет соответствие пароля пользователя хэшу пароля в базе данных.
    /// </summary>
    /// <param name="user">Пользователь, для которого требуется проверить пароль.</param>
    /// <param name="password">Пароль, который требуется проверить.</param>
    /// <returns>Значение true, если пароль совпадает с хэшем пароля пользователя, в противном случае - false.</returns>
    public static bool CheckPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.HashPassword);
    }
}
