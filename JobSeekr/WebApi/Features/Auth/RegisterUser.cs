using Mapster;
using System.ComponentModel.DataAnnotations;
using WebApi.Contract.Request;
using WebApi.Contract.Response;

namespace WebApi.Features.Auth;

public static class RegisterUser
{
    public record Command : IRequest<Result<UserResponse>>
    {
        public required string Email { get; init; }

        public required string Password { get; init; }
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
            //if (ValidationException validationException)
            //{
            //    _logger.LogError()
            //}
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
            .WithSummary("Регистрация")
            .WithDescription("Чтобы получит доступ к endpointom нужно зарегистрироваться")
            .Produces<Result>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}