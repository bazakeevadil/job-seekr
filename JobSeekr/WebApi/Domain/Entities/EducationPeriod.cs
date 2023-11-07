using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

public class EducationPeriod
{
    public long Id { get; set; }
    public long ResumeId { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength (100)]
    public string? Degree { get; set; }

    [StringLength(500)]
    public required string City { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
