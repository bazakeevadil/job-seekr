﻿namespace WebApi.Features.Resumes;

public static class AcceptResume
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


            resume.IsRejected = false;

            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}

public class AcceptResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume/accept",
            async (IMediator mediator, long id) =>
            {
                var request = new AcceptResume.Command
                {
                    Id = id,
                };

                var result = await mediator.Send(request);

                return Results.Ok(result);
            })
            .WithSummary("Принять")
            .WithDescription("Принять резюме пользователя")
            .Produces<Result>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
