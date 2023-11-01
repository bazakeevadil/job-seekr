using Mapster;

namespace WebApi.Features.Auth;

public static class RegisterUser
{
    public record Command : IRequest<Result<UserDto>>
    {
        public required string Email { get; init; }

        public required string Password { get; init; }
    }

    internal class Handler : IRequestHandler<Command, Result<UserDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is not null)
                return Result.Bad<UserDto>("Пользователь с таким адресом почты уже существует.");

            user = new User
            {
                Email = request.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = Role.User
            };

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var response = user.Adapt<UserDto>();

            return response;
        }
    }
}
