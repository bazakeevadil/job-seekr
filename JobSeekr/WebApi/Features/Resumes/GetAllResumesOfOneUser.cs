using FluentValidation;
using System.Security.Claims;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

/// <summary>
/// Класс, представляющий точку входа для получения всех своих резюме.
/// </summary>
public class GetAllResumesOfOneUserEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршрут для обработки GET-запросов по получению всех резюме текушего пользователя.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/resume/own",
            async (IMediator mediator, HttpContext httpContext) =>
            {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var query = new GetAllResumesOfOneUser.Query
                {
                    UserId = userId,
                };

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Получение текущего пользователя резюме")
            .WithDescription("Получает все резюме текушего пользователя")
            .Produces<List<ResumeResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий запрос на получение всех резюме текущего пользователя.
/// </summary>
public static class GetAllResumesOfOneUser
{
    /// <summary>
    /// Запись запроса на получение всех резюме текущего пользователя.
    /// </summary>
    public record Query : IRequest<Result<List<ResumeResponse>>>
    {
        public required long UserId { get; init; }
    }

    /// <summary>
    /// Валидатор запроса на получение всех резюме текущего пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Query> { }

    /// <summary>
    ///  Обработчик запроса на получение всех резюме текущего пользователя.
    /// </summary>
    internal class Handler : IRequestHandler<Query, Result<List<ResumeResponse>>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех резюме текущего пользователя.
        /// </summary>
        /// <param name="request">Запрос на получение всех резюме текущего пользователя.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public async Task<Result<List<ResumeResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resumes = await _context.Resumes.Where(r => r.UserId == request.UserId).AsNoTracking().ToListAsync();
     
            var response = resumes.Adapt<List<ResumeResponse>>();

            return response;
        }
    }
}
