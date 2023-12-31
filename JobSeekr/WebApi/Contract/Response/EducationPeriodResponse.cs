﻿namespace WebApi.Contract.Response;

/// <summary>
/// Ответ для получения периода обучения
/// </summary>
public record EducationPeriodResponse
{
    public long Id { get; init; }
    public long ResumeId { get; init; }
    public required string Name { get; init; }
    public string? Degree { get; init; }
    public required string City { get; init; }
    public string? Description { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}
