using Mapster;

namespace WebApi.Features.User;

public class GetAllUser
{
    public record Query : IRequest<Result<List<UserDto>>> { }

    internal class Handler : IRequestHandler<Query, Result<List<UserDto>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Result<List<UserDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();

            var response = users.Adapt<Result<List<UserDto>>>();

            return response;
        }
    }
}
