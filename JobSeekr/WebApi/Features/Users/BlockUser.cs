using WebApi.Contract.Response;
using WebApi.Features.Resumes;
using static WebApi.Features.Resumes.UpdateResume;

namespace WebApi.Features.Users;

public static class BlockUser
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


            user.IsBlocked = true;
            
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}

public class BlockUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/user/blocked",
            async (IMediator mediator, string email) =>
            {
                var request = new BlockUser.Command
                {
                    Email = email,
                };

                var result = await mediator.Send(request);

                return Results.Ok(result);
            })
            .WithSummary("Блокировать")
            .WithDescription("Позволяет заблокировать пользователя")
            .Produces<Result>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
