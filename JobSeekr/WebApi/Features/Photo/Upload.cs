using WebApi.Shared.Helpers;

namespace WebApi.Features.Photo;

public class UploadPhotoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/photo/upload", async (IMediator mediator, IFormFile file, long resumeId, HttpContext context) =>
        {
            var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _ = long.TryParse(userIdString, out var userId);

            if (file.Length > 1024 * 1024 * 8)
                return Results.BadRequest(Result.Fail("Размер файла не должен превышать 8 мб."));

            if (!file.IsImage())
                return Results.BadRequest(Result.Fail("Файл не является корректным изображением."));

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var bytes = stream.ToArray();

            var request = new Upload.Command
            {
                Data = bytes,
                ResumeId = resumeId,
                UserId = userId,
                Type = file.ContentType,
                FileName = file.FileName,
            };

            var result = await mediator.Send(request);

            if (result.IsFailure)
                return Results.BadRequest(result);

            return Results.NoContent();
        })
         .WithTags("Photo Endpoints")
         .WithSummary("Добавление фото")
         .WithDescription("Добавить фото для текушего пользователя")
         .Produces<Result>(400)
         .WithOpenApi();
    }
}

public static class Upload
{
    public record Command : IRequest<Result>
    {
        public long ResumeId { get; init; }
        public long UserId { get; init; }
        public required byte[] Data { get; init; } = new byte[0];
        public required string Type { get; init; }
        public required string FileName { get; init; }

    }

    public class Validator : AbstractValidator<Command> { }

    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FindAsync(request.ResumeId);

            if (resume == null)
                return Result.Fail("Резюме не найдено.");


            var resumePhoto = new ResumePhoto
            {
                ResumeId = request.ResumeId,
                Data = request.Data,
                Type = request.Type,
                FileName = request.FileName,
            };

            resume.ResumePhoto = resumePhoto;

            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
