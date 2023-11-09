namespace WebApi.Features.Photo;

public class UploadPhotoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/photo/upload", async (IFormFile file) =>
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var bytes = stream.ToArray();
        });
    }
}

public static class Upload
{
    public record Command : IRequest<Result>
    {
        public long ResumeId { get; init; }
        public required byte[] Data { get; set; } = new byte[0];
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
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.ResumePhoto == request.ResumeId);

            if (resume is null)
                return Result.Fail("Резюме не найден.");

            resume.IsApproved = true;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
