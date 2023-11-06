using WebApi.Contract.Response;

namespace WebApi.Features.Users;

public class GetAllUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user",
            async (IMediator mediator) =>
            {
                var query = new GetAllUser.Query();

                var result = await mediator.Send(query);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("User Endpoints")
            .WithSummary("Получение пользователей")
            .WithDescription("Получает всех пользователей")
            .RequireAuthorization("Admin")
            .Produces<List<UserResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public class GetAllUser
{
    public record Query : IRequest<Result<List<UserResponse>>> { }

    internal class Handler : IRequestHandler<Query, Result<List<UserResponse>>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<UserResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();

            var response = users.Adapt<List<UserResponse>>();

            return response;
        }
    }
}