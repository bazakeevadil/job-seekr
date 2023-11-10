using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using WebApi.Features.Resumes;
using WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Photo;


public class GetPhotosEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/photo", async (IMediator mediator, long resumeId) =>
        {
            var query = new GetPhotos.Query
            {
                ResumeId = resumeId,
            };

            var result = await mediator.Send(query);

            if (result.IsFailure)
                return Results.BadRequest(result);

            if (result.Value is null)
                return Results.NoContent(); 

            return Results.File(result.Value.Data, result.Value.FileName, result.Value.Type);
        })
            .WithTags("Photo Endpoints")
            .WithSummary("Получение фото")
            .WithDescription("Получает фото текущего пользователя")
            .Produces<PhotoResumeResponse>(200)
            .Produces<Result>(400)
            .WithOpenApi(); ;
    }
}

public static class GetPhotos
{
    public record Query : IRequest<Result<PhotoResumeResponse>>
    {
        public long ResumeId { get; init; }
    }

    public class Handler : IRequestHandler<Query, Result<PhotoResumeResponse>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PhotoResumeResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FindAsync(request.ResumeId);

            if (resume is null)
                return Result.Fail<PhotoResumeResponse>("Резюме не найден.");

            return resume.ResumePhoto.Adapt<PhotoResumeResponse>();
        }
    }
}
