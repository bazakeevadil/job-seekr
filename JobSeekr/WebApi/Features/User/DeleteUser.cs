namespace WebApi.Features.User;

public static class DeleteUser
{
    public record Command : IRequest<Result>
    {
        public required string Email { get; init; }
    }

    internal class Handler
        : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Result.Bad<UserDto>("Пользователь с таким адресом почты не существует.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
