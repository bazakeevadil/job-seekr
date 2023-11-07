using FluentValidation;
using System.Security.Claims;

namespace WebApi.Features.Resumes;

public class PrivateResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/private",
        async (IMediator mediator, HttpContext httpContext, long id) =>
        {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var request = new PrivateResume.Command
                {
                    Id = id,
                    UserId = userId,
                };

                var result = await mediator.Send(request);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Сделать резюме приватным")
            .WithDescription("Позволяет сделать резюме пользователя приватным")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public class PrivateResume
{
    public record Command : IRequest<Result>
    {
        public long Id { get; init; }
        public required long UserId { get; init; }
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
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id
            && r.UserId == request.UserId);

            if (resume is null)
                return Result.Fail("Резюме не найден.");


            resume.Status = Status.Private;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
