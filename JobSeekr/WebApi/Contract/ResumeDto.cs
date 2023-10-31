using WebApi.Domain.Enums;

namespace WebApi.Contract;

public record ResumeDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public required string FullName { get; init; }
    public required string ProgrammingLanguage { get; init; }
    public required string LanguageLevel { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public string? Links { get; init; }
    public required Status Status { get; init; }
    public required string Skills { get; init; }

    public List<EducationPeriodDto> EducationPeriods { get; init; } = new();
    public List<WorkPeriodDto> WorkPeriods { get; init; } = new();
}
