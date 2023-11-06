using System.Security.Claims;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public class DeleteResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/resume/{id}",
            async (IMediator mediator, HttpContext httpContext, long id) =>
            {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var command = new DeleteResume.Command
                {
                    Id = id,
                    UserId = userId,
                };

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Удалить резюме")
            .WithDescription("Удалить резюме текущего пользователя по ID")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class DeleteResume
{
    public record Command : IRequest<Result<ResumeResponse>>
    {
        public required long Id { get; init; }
        public required long UserId { get; init; }
    }

    internal class Handler
        : IRequestHandler<Command, Result<ResumeResponse>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ResumeResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r =>
                r.Id == request.Id
                && r.UserId == request.UserId);

            if (resume == null)
                return Result.Fail<ResumeResponse>("Резюме не найдено.");

            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync();

            return Result.Ok<ResumeResponse>();
        }
    }
}