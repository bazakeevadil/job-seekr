namespace WebApi.Contract.Response;

/// <summary>
/// Ответ для получения резюме
/// </summary>
public record ResumeResponse
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public required bool IsApproved { get; set; }
    public required string FullName { get; init; }
    public required string ProgrammingLanguage { get; init; }
    public required string LanguageLevel { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public string? Links { get; init; }
    public required Status Status { get; init; }
    public required string Skills { get; init; }

    public List<EducationPeriodResponse> EducationPeriods { get; init; } = new();
    public List<WorkPeriodResponse> WorkPeriods { get; init; } = new();
}
