using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

public class Resume
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public bool IsRejected { get; set; }

    [StringLength(50)]
    public required string FullName {  get; set; }

    [StringLength(100)]
    public required string ProgrammingLanguage {  get; set; }

    [StringLength(100)]
    public required string LanguageLevel {  get; set; }

    [StringLength(50)]
    public required string Country {  get; set; }

    [StringLength(50)]
    public required string City {  get; set; }

    [StringLength(200)]
    public string? Links { get; set; }


    public required Status Status { get; set; }

    [StringLength(300)]
    public required string Skills { get; set; }

    public List<EducationPeriod> EducationPeriods { get; set; } = new();
    public List<WorkPeriod> WorkPeriods { get; set; } = new();
    public User? User { get; set; }
}
