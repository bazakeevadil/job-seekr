namespace WebApi.Features.Users;

/// <summary>
/// Класс, отвечающий за обработку запросов на разблокировку пользователя.
/// </summary>
public class UnblockUserEndpoint : ICarterModule
{
    /// <summary>
    /// Метод, добавляющий маршруты для разблокировки пользователя.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/user/unblock",
            async (IMediator mediator, string email) =>
            {
                //Создание команды разблокировки пользователя
                var request = new UnblockUser.Command
                {
                    Email = email,
                };
                //Отправка команды разблокировки пользователя
                var result = await mediator.Send(request);
                //Проверка результата и возвращение соответствующего ответа
                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("User Endpoints")
            .WithSummary("Разблокировать")
            .WithDescription("Админ может разблокировать заблокированного пользователя")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Команда разблокировки пользователя.
/// </summary>
public static class UnblockUser
{
    /// <summary>
    /// Запись команды разблокировки пользователя.
    /// </summary>
    public record Command : IRequest<Result>
    {
        //Email пользователя.
        public required string Email { get; init; }
    }

    /// <summary>
    /// Валидатор команды разблокировки пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command>
    {
        // Конструктор валидатора.
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Почта не соответствует формату.");
        }
    }

    /// <summary>
    /// Обработчик команды разблокировки пользователя.
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
        /// Обработка команды разблокировки пользователя.
        /// </summary>
        /// <param name="request">Команда разблокировки пользовтеля.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат операции разблокировки пользователя.</returns>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            // Поиск пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
                return Result.Fail("Пользователь не найден.");

            // Разблокировка пользователя
            user.IsBlocked = false;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
