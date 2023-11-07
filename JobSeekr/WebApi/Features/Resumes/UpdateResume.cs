using FluentValidation;
using System.Security.Claims;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public class UpdateResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/resume",
            async (IMediator mediator, HttpContext httpContext, UpdateResume.Props data, long id) =>
            {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var request = new UpdateResume.Command
                {
                    Id = id,
                    UserId = userId,
                    Props = data,
                };

                var result = await mediator.Send(request);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.NoContent();
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Изменить резюме")
            .WithDescription("Изменить резюме текушего пользователя")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class UpdateResume
{
    public record Command : IRequest<Result>
    {
        public long Id { get; init; }
        public required long UserId { get; init; }
        public required Props Props { get; init; }
    }

    public record Props
    {
        public required string FullName { get; init; }
        public required string ProgrammingLanguage { get; init; }
        public required string LanguageLevel { get; init; }
        public required string Country { get; init; }
        public required string City { get; init; }
        public string? Links { get; init; }
        public required string Skills { get; init; }

        public List<EducationPeriodResponse> EducationPeriods { get; init; } = new();
        public List<WorkPeriodResponse> WorkPeriods { get; init; } = new();

    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Props)
                .NotNull()
                .NotEmpty();
            When(c => c.Props is not null, () =>
            {
                RuleFor(c => c.Props.FullName).NotNull();
                RuleFor(c => c.Props.ProgrammingLanguage).NotNull();
                RuleFor(c => c.Props.LanguageLevel).NotNull();
                RuleFor(c => c.Props.Country).NotNull();
                RuleFor(c => c.Props.City).NotNull();
                RuleFor(c => c.Props.Skills).NotNull();
                RuleFor(c => c.Props.EducationPeriods).NotNull();
                RuleFor(c => c.Props.WorkPeriods).NotNull();

            });
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
            var user = await _context.Resumes.FirstOrDefaultAsync(r =>
                r.Id == request.Id
                && r.UserId == request.UserId);

            if (user is null)
                return Result.Fail("Пользователь не найден.");

            _context.Update(user);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
