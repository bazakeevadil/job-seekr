using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

/// <summary>
/// Класс, представляющий резюме.
/// </summary>
public class Resume
{
    //Уникальный идентификатор резюме.
    public long Id { get; set; }

    //Идентификатор пользователя, связанного с резюме.
    public long UserId { get; set; }

    //Флаг, указывающий, является ли резюме одобренным.
    public bool IsApproved { get; set; }

    //Полное имя соискателя.
    [StringLength(50)]
    public required string FullName {  get; set; }

    //Программный язык, которым владеет соискатель.
    [StringLength(100)]
    public required string ProgrammingLanguage {  get; set; }

    //Уровень владения языком соискателем.
    [StringLength(100)]
    public required string LanguageLevel {  get; set; }

    //Страна проживания соискателя.
    [StringLength(50)]
    public required string Country {  get; set; }

    //Город проживания соискателя.
    [StringLength(50)]
    public required string City {  get; set; }

    //Ссылки на сот сети.
    [StringLength(200)]
    public string? Links { get; set; }

    //Статус резюме.
    public required Status Status { get; set; }

    //Навыки соискателя.
    [StringLength(300)]
    public required string Skills { get; set; }

    //Список периодов образования соискателя.
    public List<EducationPeriod> EducationPeriods { get; set; } = new();

    //Список периодов работы соискателя.
    public List<WorkPeriod> WorkPeriods { get; set; } = new();

    //Пользователь, связанный с резюме.
    public User? User { get; set; }
}