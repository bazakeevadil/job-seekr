using System.Security.Claims;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public class GetAllResumesOfOneUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/resume/own",
            async (IMediator mediator, HttpContext httpContext) =>
            {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var query = new GetAllResumesOfOneUser.Query
                {
                    UserId = userId,
                };

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Получение резюме")
            .WithDescription("Получает все резюме текушего пользователя")
            .Produces<List<ResumeResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class GetAllResumesOfOneUser
{
    public record Query : IRequest<Result<List<ResumeResponse>>>
    {
        public required long UserId { get; init; }
    }

    internal class Handler : IRequestHandler<Query, Result<List<ResumeResponse>>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ResumeResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resumes = await _context.Resumes.FirstOrDefaultAsync(r => r.UserId == request.UserId);

            var response = resumes.Adapt<List<ResumeResponse>>();

            return response;
        }
    }
}
