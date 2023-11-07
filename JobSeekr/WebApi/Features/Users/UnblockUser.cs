using FluentValidation;

namespace WebApi.Features.Users;

public class UnblockUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/user/unblock",
            async (IMediator mediator, string email) =>
            {
                var request = new UnblockUser.Command
                {
                    Email = email,
                };

                var result = await mediator.Send(request);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("User Endpoints")
            .WithSummary("Разблокировать")
            .WithDescription("Админ может разблокировать заблокированного пользователя")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class UnblockUser
{
    public record Command : IRequest<Result>
    {
        public required string Email { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c).NotNull();

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Почта не соответствует формату.");
        }
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
