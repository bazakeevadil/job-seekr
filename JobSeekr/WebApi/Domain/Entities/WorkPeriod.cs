namespace WebApi.Domain.Entities;

public class WorkPeriod
{
    public long Id { get; set; }
    public long ResumeId { get; set; }
    public required string Position { get; set; }
    public required string Employer { get; set; }
    public required string City { get; set; }
    public string? Description { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
