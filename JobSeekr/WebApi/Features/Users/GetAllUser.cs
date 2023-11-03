using Mapster;
using WebApi.Contract.Request;
using WebApi.Contract.Response;
using WebApi.Features.Resumes;

namespace WebApi.Features.Users;

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

public class GetAllUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user",
            async (IMediator mediator) =>
            {
                var query = new GetAllUser.Query();

                var response = await mediator.Send(query);

                return Results.Ok(response);
            })
            .WithSummary("Получение пользователей")
            .WithDescription("Получает всех пользователей")
            .Produces<List<UserResponse>>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}