namespace WebApi.Features.Users;

public static class UnlockUser
{
    public record Command : IRequest<Result>
    {
        public required string Email { get; set; }
    }

    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
                return Result.Fail("Пользователь не найден.");


            user.IsBlocked = false;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}

public class UnlockUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/user/unlock",
            async (IMediator mediator, string email) =>
            {
                var request = new BlockUser.Command
                {
                    Email = email,
                };

                var result = await mediator.Send(request);

                return Results.Ok(result);
            })
            .WithSummary("Разблокировать")
            .WithDescription("Админ может разблокировать заблокированного пользователя")
            .Produces<Result>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
