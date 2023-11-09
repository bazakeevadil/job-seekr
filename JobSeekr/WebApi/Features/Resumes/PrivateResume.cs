using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий модуль для установки резюме в приватный режим.
/// </summary>
public class PrivateResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Метод для добавления маршрутов.
    /// </summary>
    /// <param name="app">Построитель конечных точек маршрута.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/private",
        async (IMediator mediator, HttpContext httpContext, long id) =>
        {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var request = new PrivateResume.Command
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
            .WithSummary("Сделать резюме приватным")
            .WithDescription("Позволяет сделать резюме пользователя приватным")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий команду для установки резюме в приватный режим.
/// </summary>
public class PrivateResume
{
    /// <summary>
    /// Запись команды для установки резюме в приватный режим.
    /// </summary>
    public record Command : IRequest<Result>
    {
        //Уникальный идентификатор резюме.
        public long Id { get; init; }
        //Идентификатор пользователя.
        public required long UserId { get; init; }
    }

    /// <summary>
    /// Валидатор команды для установки резюме в приватный режим.
    /// </summary>
    public class Validator : AbstractValidator<Command> { }

    /// <summary>
    /// Обработчик команды для установки резюме в приватный режим.
    /// </summary>
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Конструктор обработчика команды для установки резюме в приватный режим.
        /// </summary>
        /// <param name="context">Контекст приложения.</param>
        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод обработки команды для установки резюме в приватный режим.
        /// </summary>
        /// <param name="request">Команда для установки резюме в приватный режим.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id
            && r.UserId == request.UserId);

            if (resume is null)
                return Result.Fail("Резюме не найден.");


            resume.Status = Status.Private;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}