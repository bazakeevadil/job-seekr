using Mapster;
using WebApi.Contract.Request;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public static class GetAllResume
{
    public record Query : IRequest<Result<List<ResumeResponse>>> { }

    internal class Handler : IRequestHandler<Query, Result<List<ResumeResponse>>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ResumeResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resumes = await _context.Resumes.AsNoTracking().ToListAsync();

            var response = resumes.Adapt<List<ResumeResponse>>();

            return response;
        }
    }
}

public class GetAllResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/resume",
            async (IMediator mediator) =>
            {
                var query = new GetAllResume.Query();

                var response = await mediator.Send(query);

                return Results.Ok(response);
            })
            .WithSummary("Получение резюме")
            .WithDescription("Получает все резюме текушего пользователя")
            .Produces<List<ResumeResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}