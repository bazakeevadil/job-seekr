using FluentValidation;
using WebApi.Contract.Request;
using WebApi.Contract.Response;

namespace WebApi.Features.Auth;

public class RegisterUserEndpoint : ICarterModule
{
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

public static class RegisterUser
{
    public record Command : IRequest<Result<UserResponse>>
    {
        public required string Email { get; init; }

        public required string Password { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(c => c.Email)
                .NotEmpty()
                .Length(1, 200)
                .EmailAddress().WithMessage("Почта не соответствует формату.");

            RuleFor(c => c.Password)
                .NotEmpty()
                .MinimumLength(4).WithMessage("Пароль должен содержать не меньше 4 символов.")
                .Matches(@"[0-9]+").WithMessage("Пароль должен содержать цифры.")
                .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]").WithMessage("Пароль должен содержать специальные символы.");
        }
    }

    internal class Handler : IRequestHandler<Command, Result<UserResponse>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(AppDbContext appDbContext, ILogger<Handler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

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