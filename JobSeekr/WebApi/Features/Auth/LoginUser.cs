using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Contract.Request;
using WebApi.Shared.Helpers;

namespace WebApi.Features.Auth;

/// <summary>
/// Endpoint авторизации пользователя
/// </summary>
public class LoginUserEndpoint : ICarterModule
{
    /// <summary>
    /// Добавляет маршруты для модуля авторизации.
    /// </summary>
    /// <param name="app">Построитель маршрутов.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login",
            async (IMediator mediator, LoginUserRequest request) =>
            {
                var query = request.Adapt<LoginUser.Query>();

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .AllowAnonymous()
            .WithTags("Auth Endpoints")
            .WithSummary("Аутентификация")
            .WithDescription("Чтобы получит доступ к endpoints нужно аутентификация чтобы понять что можно пользователю ")
            .Produces<string>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

/// <summary>
/// Класс, представляющий запрос на авторизацию пользователя.
/// </summary>
public static class LoginUser
{
    /// <summary>
    /// Запрос на авторизацию пользователя.
    /// </summary>
    public record Query : IRequest<Result<string>>
    {
        /// <summary>
        /// Электронная почта пользователя.
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public required string Password { get; init; }
    }

    /// <summary>
    /// Валидатор запроса на авторизацию пользователя.
    /// </summary>
    public class Validator : AbstractValidator<Query>
    {
        //Конструктор валидатора.
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress();

            RuleFor(c => c.Password)
                .NotEmpty();
        }
    }

    /// <summary>
    /// Обработчик запроса на авторизацию пользователя.
    /// </summary>
    internal class Handler : IRequestHandler<Query, Result<string>>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public Handler(AppDbContext appDbContext, IConfiguration configuration)
        {
            _context = appDbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Обрабатывает запрос на авторизацию пользователя.
        /// </summary>
        /// <param name="request">Запрос на авторизацию пользователя.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Результат авторизации с токеном доступа.</returns>
        public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null || !AuthHelper.CheckPassword(user, request.Password))
                return Result.Fail<string>("Пользователь не найден или пароль указан неверно.");

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

            var tokenString = GetTokenString(claims, DateTime.UtcNow.AddMinutes(30));

            return tokenString;
        }

        /// <summary>
        /// Генерирует строку JWT-токена на основе указанных утверждений и времени истечения срока действия.
        /// </summary>
        /// <param name="claims">Утверждения токена.</param>
        /// <param name="exp">Время истечения срока действия токена.</param>
        /// <returns>Строка JWT-токена.</returns>
        private string GetTokenString(List<Claim> claims, DateTime exp)
        {
            var key = _configuration["JWT_TOKEN"]
                ?? throw new Exception("Секретный ключ для генерации JWT не найден в файле кофигурации.");
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: exp,
                signingCredentials: new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256));

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }
    }
}