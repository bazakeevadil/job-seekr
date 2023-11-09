namespace WebApi.Features.Users;

/// <summary>
/// Класс, отвечающий за обработку запросов на удаление пользователя.
/// </summary>
public class DeleteUserEndpoint : ICarterModule
{
    /// <summary>
    /// Метод, добавляющий маршруты для удаления пользователя.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/user/{email}",
            async (IMediator mediator, HttpContext httpContext, string email) =>
            {
                // Создание команды удаления пользователя
                var command = new DeleteUser.Command
                {
                    Email = email
                };
                // Отправка команды удаления пользователя
                var result = await mediator.Send(command);
                // Проверка результата и возвращение соответствующего ответа
                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("User Endpoints")
            .WithSummary("Удалить пользователя")
            .WithDescription("Удалить пользователя по Email")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Команда удаления пользователя.
/// </summary>
public static class DeleteUser
{
    /// <summary>
    /// Запись команды удаления пользователя.
    /// </summary>
    public record Command : IRequest<Result>
    {
        /// <summary>
        /// Email пользователя.
        /// </summary>
        public required string Email { get; init; }
    }

    /// <summary>
    /// Валидатор команды удаления пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command>
    {
        //Конструктор валидатора.
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Почта не соответствует формату.");
        }
    }

    /// <summary>
    /// Обработчик команды удаления пользователя.
    /// </summary>
    internal class Handler
        : IRequestHandler<Command, Result>
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
        /// Обработка команды удаления пользователя.
        /// </summary>
        /// <param name="request">Команда удаления пользователя.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат операции удаления пользователя.</returns>
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // Поиск пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Result.Fail<UserResponse>("Пользователь с таким адресом почты не существует.");
            // Удаление пользователя
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
