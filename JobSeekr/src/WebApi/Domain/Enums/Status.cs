namespace WebApi.Domain.Enums;

/// <summary>
/// Перечисление, определяющее статусы.
/// </summary>
public enum Status
{
    //Ожидает подтверждения.
    Pending = 1,

    //Приватный статус.
    Private = 2,

    //Публичный статус.
    Public = 3,
}