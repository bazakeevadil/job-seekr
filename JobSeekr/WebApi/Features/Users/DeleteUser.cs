using System.Security.Claims;
using WebApi.Contract.Response;
using WebApi.Features.Resumes;

namespace WebApi.Features.Users;

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
                return Result.Fail<UserResponse>("Пользователь с таким адресом почты не существует.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}

public class DeleteUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/user/{email}",
            async (IMediator mediator, HttpContext httpContext, string email) =>
            {
                var command = new DeleteUser.Command
                {
                    Email = email
                };

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok();
            })
            .WithSummary("Удалить пользователя")
            .WithDescription("Удалить пользователя по Email")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
