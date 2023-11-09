namespace WebApi.Domain.Entities;

public class ResumePhoto
{
    public long ResumeId { get; set; }
    public required byte[] Data { get; set; } = new byte[0];

    public Resume? Resume { get; set; }
}
