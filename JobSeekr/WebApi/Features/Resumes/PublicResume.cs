using FluentValidation;
using System.Security.Claims;

namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий точку входа для установки публичного статуса резюме пользователя.
/// </summary>
public class PublicResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршруты для API.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/public",
        async (IMediator mediator, HttpContext httpContext, long id) =>
        {
            var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _ = long.TryParse(userIdString, out var userId);

            var request = new PublicResume.Command
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
            .WithSummary("Сделать резюме публичным")
            .WithDescription("Позволяет сделать резюме пользователя публичным")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий команду для установки резюме в публичный режим.
/// </summary>
public class PublicResume
{
    /// <summary>
    /// Команда для установки публичного статуса резюме пользователя.
    /// </summary>
    public record Command : IRequest<Result>
    {
        //Идентификатор резюме пользователя для установки публичного статуса.
        public long Id { get; init; }
        //Идентификатор пользователя, чей резюме будет установлено публичным.
        public required long UserId { get; init; }
    }

    /// <summary>
    /// Валидатор команды установки публичного статуса резюме пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command> { }

    /// <summary>
    /// Обработчик команды установки публичного статуса резюме пользователя.
    /// </summary>
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает команду установки публичного статуса резюме пользователя.
        /// </summary>
        /// <param name="request">Команда установки публичного статуса резюме пользователя.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id
            && r.UserId == request.UserId);

            if (resume is null)
                return Result.Fail("Резюме не найден.");


            resume.Status = Status.Public;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}