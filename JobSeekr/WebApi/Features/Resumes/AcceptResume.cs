using FluentValidation;

namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, отвечающий за обработку запросов на принятие резюме пользователя.
/// </summary>
public class AcceptResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Метод, добавляющий маршруты для принятия резюме пользователя.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/accept",
            async (IMediator mediator, long id) =>
            {
                // Создание команды принятия резюме
                var request = new AcceptResume.Command
                {
                    Id = id,
                };
                // Отправка команды принятия резюме
                var result = await mediator.Send(request);
                // Проверка результата и возвращение соответствующего ответа
                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Принять")
            .WithDescription("Принять резюме пользователя")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Команда принятия резюме пользователя.
/// </summary>
public static class AcceptResume
{
    /// <summary>
    /// Запись команды принятия резюме пользователя.
    /// </summary>
    public record Command : IRequest<Result>
    {
        //Идентификатор резюме.
        public long Id { get; init; }
    }

    /// <summary>
    /// Валидатор команды принятия резюме пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command> { }

    /// <summary>
    /// Обработчик команды принятия резюме пользователя.
    /// </summary>
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Конструктор обработчика.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обработка команды принятия резюме пользователя.
        /// </summary>
        /// <param name="request">Команда принятия резюме.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат операции принятия резюме пользователя.</returns>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            // Поиск резюме по идентификатору
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id);

            if (resume is null)
                return Result.Fail("Резюме не найден.");

            // Принятие резюме
            resume.IsApproved = true;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
