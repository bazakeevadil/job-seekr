namespace WebApi.Features.Auth;

/// <summary>
/// Endpoint для регистрации пользователя.
/// </summary>
public class RegisterUserEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршруты к конечной точке.
    /// </summary>
    /// <param name="app">Построитель конечных точек</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/register",
            async (IMediator mediator, RegisterUserRequest request) =>
            {
                var command = request.Adapt<RegisterUser.Command>();

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .AllowAnonymous()
            .WithTags("Auth Endpoints")
            .WithSummary("Регистрация")
            .WithDescription("Чтобы получит доступ к endpointom нужно зарегистрироваться")
            .Produces<UserResponse>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Команда для регистрации пользователя.
/// </summary>
public static class RegisterUser
{
    public record Command : IRequest<Result<UserResponse>>
    {
        /// <summary>
        /// Получает или устанавливает адрес электронной почты пользователя.
        /// </summary>
        public required string Email { get; init; }

        /// <summary>
        /// Получает или устанавливает пароль пользователя.
        /// </summary>
        public required string Password { get; init; }
    }

    /// <summary>
    /// Валидатор для команды регистрации пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Command>
    {
        //Конструктор валидатора.
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(c => c.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(1, 200)
                .EmailAddress().WithMessage("Почта не соответствует формату.");

            RuleFor(c => c.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(4).WithMessage("Пароль должен содержать не меньше 4 символов.")
                .Matches(@"[0-9]+").WithMessage("Пароль должен содержать цифры.")
                .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]").WithMessage("Пароль должен содержать специальные символы.");
        }
    }

    /// <summary>
    /// Обработчик для команды регистрации пользователя.
    /// </summary>
    internal class Handler : IRequestHandler<Command, Result<UserResponse>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Обрабатывает команду регистрации пользователя.
        /// </summary>
        /// <param name="request">Команда регистрации пользователя.</param>
        /// <param name="cancellationToken">Токен отмены для операции</param>
        /// <returns>Результат операции регистрации пользователя.</returns>
        public async Task<Result<UserResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is not null)
                return Result.Fail<UserResponse>("Пользователь с таким адресом почты уже существует.");

            user = new User
            {
                Email = request.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsBlocked = false,
                Role = Role.User
            };

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var response = user.Adapt<UserResponse>();

            return response;
        }
    }
}