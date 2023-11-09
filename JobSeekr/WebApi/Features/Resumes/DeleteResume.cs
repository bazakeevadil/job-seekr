namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий модуль удаления резюме.
/// </summary>
public class DeleteResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Метод для добавления маршрутов.
    /// </summary>
    /// <param name="app">Построитель конечных точек маршрута.</param>
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

/// <summary>
/// Класс, представляющий команду удаления резюме.
/// </summary>
public static class DeleteResume
{
    /// <summary>
    /// Запись команды для удаления резюме.
    /// </summary>
    public record Command : IRequest<Result<ResumeResponse>>
    {
        //Уникальный идентификатор резюме.
        public required long Id { get; init; }
        //Идентификатор пользователя.
        public required long UserId { get; init; }
    }

    /// <summary>
    /// Валидатор команды удаления резюме.
    /// </summary>
    public class Validator : AbstractValidator<Command> { }

    /// <summary>
    /// Обработчик команды удаления резюме.
    /// </summary>
    internal class Handler
        : IRequestHandler<Command, Result<ResumeResponse>>
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Конструктор обработчика команды удаления резюме.
        /// </summary>
        /// <param name="context">Контекст приложения.</param>
        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод обработки команды удаления резюме.
        /// </summary>
        /// <param name="request">Команда удаления резюме.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
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