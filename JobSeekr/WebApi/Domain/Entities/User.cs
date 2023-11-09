using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

/// <summary>
/// Представляет пользователя в системе.
/// </summary>
public class User
{
    //Идентификатор пользователя.
    public long Id { get; set; }

    //Email пользователя.
    [StringLength(150)]
    public required string Email { get; set; }

    //Хэшированный пароль пользователя.
    [StringLength(200)]
    public required string HashPassword { get; set; }

    //Флаг, указывающий на заблокированность пользователя.
    public required bool IsBlocked { get; set; }

    //Роль пользователя.
    public Role Role { get; set; }

    //Список резюме пользователя.
    public List<Resume> Resumes { get; set; } = new();
}
