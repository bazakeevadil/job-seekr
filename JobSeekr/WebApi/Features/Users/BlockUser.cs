using FluentValidation;

namespace WebApi.Features.Users;

/// <summary>
/// Класс, отвечающий за обработку запросов на блокировку пользователя.
/// </summary>
public class BlockUserEndpoint : ICarterModule
{
    /// <summary>
    /// Метод, добавляющий маршруты для блокировки пользователя.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/user/blocked",
            async (IMediator mediator, string email) =>
            {
                //Создание команды блокировки пользователя
                var request = new BlockUser.Command
                {
                    Email = email,
                };
                //Отправка команды блокировки пользователя
                var result = await mediator.Send(request);
                //Проверка результата и возвращение соответствующего ответа
                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("User Endpoints")
            .WithSummary("Блокировать")
            .WithDescription("Позволяет заблокировать пользователя")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Команда блокировки пользователя.
/// </summary>
public static class BlockUser
{
    /// <summary>
    ///  Запись команды блокировки пользователя.
    /// </summary>
    public record Command : IRequest<Result>
    {
        /// <summary>
        /// Email пользователя.
        /// </summary>
        public required string Email { get; init; }
    }

    /// <summary>
    /// Валидатор команды блокировки пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command>
    {
        //Конструктор валидатора.
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }

    /// <summary>
    /// Обработчик команды блокировки пользователя.
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
        /// Обработка команды блокировки пользователя.
        /// </summary>
        /// <param name="request">Команда блокировки пользователя.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат операции блокировки пользователя.</returns>
        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            //Поиск пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
                return Result.Fail("Пользователь не найден.");

            //Блокировка пользователя
            user.IsBlocked = true;
            
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
