namespace WebApi.Features.Resumes;

public class RejectResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/reject",
            async (IMediator mediator, long id) =>
            {
                var request = new RejectResume.Command
                {
                    Id = id,
                };

                var result = await mediator.Send(request);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Отклонить")
            .WithDescription("Позволяет отклонить резюме")
            .RequireAuthorization("Admin")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class RejectResume
{
    public record Command : IRequest<Result>
    {
        public long Id { get; set; }
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
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == request.Id);

            if (resume is null)
                return Result.Fail("Резюме не найден.");


            resume.IsRejected = true;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
