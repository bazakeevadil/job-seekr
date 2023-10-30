namespace WebApi.Entities;

public class Resume
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public Status Status { get; set; }

    public required string Social { get; set; }
    public required string Education { get; set; }
    public required string Skills { get; set; }
    public required string Work {  get; set; }


    public required User User { get; set; }
}
