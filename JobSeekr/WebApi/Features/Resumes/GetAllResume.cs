using FluentValidation;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public class GetAllResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/resume",
            async (IMediator mediator) =>
            {
                var query = new GetAllResume.Query();

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Получение резюме")
            .WithDescription("Получает резюме всех пользователей")
            .RequireAuthorization("Admin")
            .Produces<List<ResumeResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class GetAllResume
{
    public record Query : IRequest<Result<List<ResumeResponse>>> { }

    public class Validator : AbstractValidator<Query> { }

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