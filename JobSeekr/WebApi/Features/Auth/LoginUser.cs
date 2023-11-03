using Mapster;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Contract.Request;
using WebApi.Shared.Helpers;

namespace WebApi.Features.Auth;

public static class LoginUser
{
    public record Query : IRequest<Result<string>>
    {
        public required string Email { get; init; }

        public required string Password { get; init; }
    }

    internal class Handler : IRequestHandler<Query, Result<string>>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public Handler(AppDbContext appDbContext, IConfiguration configuration)
        {
            _context = appDbContext;
            _configuration = configuration;
        }

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

        private string GetTokenString(List<Claim> claims, DateTime exp)
        {
            var key = _configuration["JWT"] ?? throw new Exception("JWT not found");
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

public class LoginUserEndpoint : ICarterModule
{
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
            .WithSummary("Аутентификация")
            .WithDescription("Чтобы получит доступ к endpoints нужно аутентификация чтобы понять что можно пользователю ")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
