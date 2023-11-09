using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

/// <summary>
/// Класс, представляющий период обучения.
/// </summary>
public class EducationPeriod
{
    //Уникальный идентификатор периода обучения.
    public long Id { get; set; }

    //Идентификатор резюме, к которому относится этот период обучения.
    public long ResumeId { get; set; }

    //Название учебного заведения или курса.
    [StringLength(100)]
    public required string Name { get; set; }

    //Степень, полученная по завершении обучения.
    [StringLength (100)]
    public string? Degree { get; set; }

    //Название города.
    [StringLength(500)]
    public required string City { get; set; }

    //Описание периода обучения.
    [StringLength(200)]
    public string? Description { get; set; }

    //Дата начала периода обучения.
    public DateTime? From { get; set; }

    //Дата окончания периода обучения.
    public DateTime? To { get; set; }
}