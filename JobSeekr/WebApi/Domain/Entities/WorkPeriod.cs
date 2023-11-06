using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

public class WorkPeriod
{
    public long Id { get; set; }
    public long ResumeId { get; set; }

    [StringLength(100)]
    public required string Position { get; set; }

    [StringLength(50)]
    public required string Employer { get; set; }

    [StringLength(50)]
    public required string City { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
