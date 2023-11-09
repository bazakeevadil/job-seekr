using FluentValidation;

namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий точку входа для отклонения резюме.
/// </summary>
public class RejectResumeEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршруты для API.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/reject",
            async (IMediator mediator, long id) =>
            {
                var request = new RejectResume.Command
                {
                    Id = id,
                };

                var result = await mediator.Send(request);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Отклонить")
            .WithDescription("Позволяет отклонить резюме")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Команда отклонения резюме пользователя.
/// </summary>
public static class RejectResume
{
    /// <summary>
    /// Команда для отклонения резюме.
    /// </summary>
    public record Command : IRequest<Result>
    {
        //Идентификатор резюме для отклонения.
        public long Id { get; init; }
    }

    /// <summary>
    /// Валидатор команды отклонения резюме.
    /// </summary>
    public class Validator : AbstractValidator<Command> { }

    /// <summary>
    /// Обработчик команды отклонения резюме.
    /// </summary>
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;


        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает команду отклонения резюме.
        /// </summary>
        /// <param name="request">Команда отклонения резюме.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id);

            if (resume is null)
                return Result.Fail("Резюме не найден.");


            resume.IsApproved = false;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
