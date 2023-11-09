namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий точку входа для установки статуса резюме пользователя.
/// </summary>
public class ChangeStatusResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршруты для API.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/status/{id}",
        async (IMediator mediator, HttpContext httpContext, long id, Status status) =>
        {
            var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _ = long.TryParse(userIdString, out var userId);

            var request = new ChangeStatusResume.Command
            {
                Id = id,
                UserId = userId,
                Status = status,
            };

            var result = await mediator.Send(request);
            
            if (result.IsFailure)
                return Results.BadRequest(result);
            
            return Results.NoContent();
        })
            .WithTags("Resume Endpoints")
            .WithSummary("Сделать резюме публичным")
            .WithDescription("Позволяет сделать резюме пользователя публичным")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий команду для установки статуса резюме.
/// </summary>
public class ChangeStatusResume
{
    /// <summary>
    /// Команда для установки статуса резюме пользователя.
    /// </summary>
    public record Command : IRequest<Result>
    {
        //Идентификатор резюме пользователя для установки статуса.
        public long Id { get; init; }
        //Идентификатор пользователя.
        public required long UserId { get; init; }
        //Статус резюме.
        public Status Status { get; init; }
    }

    /// <summary>
    /// Валидатор команды установки статуса резюме пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command> { }

    /// <summary>
    /// Обработчик команды установки статуса резюме пользователя.
    /// </summary>
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает команду установки статуса резюме пользователя.
        /// </summary>
        /// <param name="request">Команда установки статуса резюме пользователя.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id
            && r.UserId == request.UserId);

            if (resume is null)
                return Result.Fail("Резюме не найдено.");

            if (!resume.IsApproved )
                return Result.Fail("Резюме все еще на проверке.");

            if (request.Status == Status.Pending)
                return Result.Fail("Нельзя установить статус 'в ожидании'.");

            resume.Status = request.Status;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}