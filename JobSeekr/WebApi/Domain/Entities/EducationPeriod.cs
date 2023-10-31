namespace WebApi.Domain.Entities;

public class EducationPeriod
{
    public long Id { get; set; }
    public long ResumeId { get; set; }
    public required string Name { get; set; }
    public string? Degree { get; set; }
    public required string City { get; set; }
    public string? Description { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
