namespace WebApi.Contract.Response;

public record WorkPeriodResponse
{
    public long Id { get; init; }
    public long ResumeId { get; init; }
    public required string Position { get; init; }
    public required string Employer { get; init; }
    public required string City { get; init; }
    public string? Description { get; init; }
    public DateTime From { get; init; }
    public DateTime? To { get; init; }
}
