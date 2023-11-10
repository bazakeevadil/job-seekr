using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

/// <summary>
/// Класс, представляющий фото в резюме.
/// </summary>
public class ResumePhoto
{
    //Идентификатор резюме, к которому относится это фото.
    public long ResumeId { get; set; }

    //Массив байтов фото резюме
    public required byte[] Data { get; set; } = new byte[0];

    //Тип файла фото
    [StringLength(50)]
    public required string Type { get; set; }

    //Имя файла фото
    [StringLength(50)]
    public required string FileName { get; set; }

    //Список резюме пользователя
    public Resume? Resume { get; set; }
}
