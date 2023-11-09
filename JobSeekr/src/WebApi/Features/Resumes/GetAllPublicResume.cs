namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий точку входа для получения всех публичных резюме.
/// </summary>
public class GetAllPublicResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршрут для обработки GET-запросов по получению всех публичных резюме.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/resume",
            async (IMediator mediator) =>
            {
                var query = new GetAllPublicResume.Query();

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Получение публичных резюме")
            .WithDescription("Получает публичные резюме всех пользователей")
            .RequireAuthorization("Admin")
            .Produces<List<ResumeResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий запрос на получение всех публичных резюме.
/// </summary>
public static class GetAllPublicResume
{
    /// <summary>
    /// Запись запроса на получение всех публичных резюме.
    /// </summary>
    public record Query : IRequest<Result<List<ResumeResponse>>> { }

    /// <summary>
    /// Валидатор запроса на получение всех публичных резюме.
    /// </summary>
    public class Validator : AbstractValidator<Query> { }

    /// <summary>
    ///  Обработчик запроса на получение всех публичных резюме.
    /// </summary>
    internal class Handler : IRequestHandler<Query, Result<List<ResumeResponse>>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех публичных резюме.
        /// </summary>
        /// <param name="request">Запрос на получение всех публичных резюме.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result<List<ResumeResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resumes = await _context.Resumes.Where(r => r.Status == Status.Public).AsNoTracking().ToListAsync();

            var response = resumes.Adapt<List<ResumeResponse>>();

            return response;
        }
    }
}