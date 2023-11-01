namespace WebApi.Domain.Entities;

public class Resume
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string FullName {  get; set; }
    public required string ProgrammingLanguage {  get; set; }
    public required string LanguageLevel {  get; set; }
    public required string Country {  get; set; }
    public required string City {  get; set; }
    public string? Links { get; set; }


    public required Status Status { get; set; }
    public required string Skills { get; set; }

    public List<EducationPeriod> EducationPeriods { get; set; } = new();
    public List<WorkPeriod> WorkPeriods { get; set; } = new();
    public User? User { get; set; }
}
