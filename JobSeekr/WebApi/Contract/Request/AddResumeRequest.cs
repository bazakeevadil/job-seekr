using WebApi.Contract.Response;

namespace WebApi.Contract.Request;

public record AddResumeRequest
{
    public required string FullName { get; init; }
    public required string ProgrammingLanguage { get; init; }
    public required string LanguageLevel { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public string? Links { get; init; }
    public required string Skills { get; init; }

    public List<EducationPeriodResponse> EducationPeriods { get; init; } = new();
    public List<WorkPeriodResponse> WorkPeriods { get; init; } = new();
}
