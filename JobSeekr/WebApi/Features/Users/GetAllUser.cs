using FluentValidation;
using WebApi.Contract.Response;

namespace WebApi.Features.Users;

/// <summary>
/// Класс, отвечающий за обработку запросов на получение всех пользователей.
/// </summary>
public class GetAllUserEndpoint : ICarterModule
{
    /// <summary>
    /// Метод, добавляющий маршруты для получения всех пользователей.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user",
            async (IMediator mediator) =>
            {
                // Создание запроса на получение всех пользователей
                var query = new GetAllUser.Query();
                // Отправка запроса на получение всех пользователей
                var result = await mediator.Send(query);
                // Проверка результата и возвращение соответствующего ответа
                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("User Endpoints")
            .WithSummary("Получение пользователей")
            .WithDescription("Получает всех пользователей")
            .RequireAuthorization("Admin")
            .Produces<List<UserResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Запрос на получение всех пользователей.
/// </summary>
public class GetAllUser
{
    /// <summary>
    /// Запись запроса на получение всех пользователей.
    /// </summary>
    public record Query : IRequest<Result<List<UserResponse>>> { }

    /// <summary>
    /// Валидатор запроса на получение всех пользователей.
    /// </summary>
    public class Validator : AbstractValidator<Query> { }

    /// <summary>
    /// Обработчик запроса на получение всех пользователей.
    /// </summary>
    internal class Handler : IRequestHandler<Query, Result<List<UserResponse>>>
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
        /// Обработка запроса на получение всех пользователей.
        /// </summary>
        /// <param name="request">Запрос на получение всех пользователей.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат операции получения всех пользователей.</returns>
        public async Task<Result<List<UserResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Получение всех пользователей без отслеживания изменений
            var users = await _context.Users.AsNoTracking().ToListAsync();
            // Преобразование пользователей в список объектов UserResponse
            var response = users.Adapt<List<UserResponse>>();

            return response;
        }
    }
}