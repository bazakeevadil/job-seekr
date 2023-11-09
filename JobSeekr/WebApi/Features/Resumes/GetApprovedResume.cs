namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий точку входа для получения проверенных резюме.
/// </summary>
public class GetApprovedResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршрут для обработки GET-запросов по получению проверенных резюме.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/resume/accept",
            async (IMediator mediator) =>
            {
                var query = new GetApprovedResume.Query();

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Получение потвержденные резюме")
            .WithDescription("Получает все потвежденные резюме")
            .RequireAuthorization("Admin")
            .Produces<List<ResumeResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий запрос на получение проверенных резюме.
/// </summary>
public static class GetApprovedResume
{
    /// <summary>
    /// Запись запроса на получение всех потвержденных резюме.
    /// </summary>
    public record Query : IRequest<Result<List<ResumeResponse>>> { }

    /// <summary>
    /// Валидатор запроса на получение проверенных резюме.
    /// </summary>
    public class Validator : AbstractValidator<Query> { }

    internal class Handler : IRequestHandler<Query, Result<List<ResumeResponse>>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает запрос на получение проверенных резюме.
        /// </summary>
        /// <param name="request">Запрос на получение проверенных резюме.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result<List<ResumeResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resumes = await _context.Resumes.Where(r => r.IsApproved == true).AsNoTracking().ToListAsync();

            var response = resumes.Adapt<List<ResumeResponse>>();

            return response;
        }
    }
}
