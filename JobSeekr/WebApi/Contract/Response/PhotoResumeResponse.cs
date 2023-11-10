namespace WebApi.Contract.Response;

/// <summary>
/// Ответ для получения фото в резюме
/// </summary>
public record PhotoResumeResponse
{
    public long ResumeId { get; init; }
    public required byte[] Data { get; init; } = new byte[0];
    public required string Type { get; init; }
    public required string FileName { get; init; }
}
