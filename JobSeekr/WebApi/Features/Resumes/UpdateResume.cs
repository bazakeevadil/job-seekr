using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public static class UpdateResume
{
    public record Command : IRequest<Result>
    {
        public required long UserId { get; init; }
        public required Props Props { get; init; }
    }

    public record Props
    {
        public required string FullName { get; init; }
        public required string ProgrammingLanguage { get; init; }
        public required string LanguageLevel { get; init; }
        public required string Country { get; init; }
        public required string City { get; init; }
        public string? Links { get; init; }
        public required string Skills { get; init; }

        public List<EducationPeriodResponse> EducationPeriods { get; init; } = new();
        public List<WorkPeriodResponse> WorkPeriods { get; init; } = new();

    }

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
            var user = await _context.Users.FindAsync(request.UserId);

            if (user is null)
                return Result.Fail("Пользователь не найден.");



            _context.Update(user);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}

public class UpdateResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/update",
            async (IMediator mediator, HttpContext httpContext, UpdateResume.Props data) =>
            {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var request = new UpdateResume.Command
                {
                    UserId = userId,
                    Props = data,
                };

                var result = await mediator.Send(request);

                return Results.Ok(result);
            })
            .WithSummary("Изменить резюме")
            .WithDescription("Изменить резюме текушего пользователя")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
