﻿using FluentValidation;
using WebApi.Contract.Response;

namespace WebApi.Features.Users;

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

                return Results.NoContent();
            })
            .WithTags("User Endpoints")
            .WithSummary("Удалить пользователя")
            .WithDescription("Удалить пользователя по Email")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class DeleteUser
{
    public record Command : IRequest<Result>
    {
        public required string Email { get; init; }
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
