using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

/// <summary>
/// Класс, представляющий рабочий период.
/// </summary>
public class WorkPeriod
{
    //Уникальный идентификатор рабочего периода.
    public long Id { get; set; }

    //Идентификатор резюме, к которому относится этот рабочий период.
    public long ResumeId { get; set; }

    //Название должности.
    [StringLength(100)]
    public required string Position { get; set; }

    //Название работодателя.
    [StringLength(50)]
    public required string Employer { get; set; }

    //Название города.
    [StringLength(50)]
    public required string City { get; set; }

    //Описание рабочего периода.
    [StringLength(200)]
    public string? Description { get; set; }

    //Дата начала рабочего периода.
    public DateTime? From { get; set; }

    //Дата окончания рабочего периода.
    public DateTime? To { get; set; }
}