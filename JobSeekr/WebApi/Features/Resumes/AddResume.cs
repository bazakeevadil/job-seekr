using FluentValidation;
using System.Security.Claims;
using WebApi.Contract.Request;
using WebApi.Contract.Response;

namespace WebApi.Features.Resumes;

public class AddResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/resume",
            async (IMediator mediator, AddResumeRequest request, HttpContext httpContext) =>
            {
                var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                _ = long.TryParse(userIdString, out var userId);

                var command = request.Adapt<AddResume.Command>();

                command = command with { UserId = userId };

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithTags("Resume Endpoints")
            .WithSummary("Создание резюме")
            .WithDescription("Создать резюме текушего пользователя")
            .Produces<ResumeResponse>(200)
            .Produces<Result>(400)
            .WithOpenApi();
    }
}

public static class AddResume
{
    public record Command : IRequest<Result<ResumeResponse>>
    {
        public long UserId { get; init; }
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
            RuleFor(c => c.FullName).NotNull().NotEmpty();
            RuleFor(c => c.ProgrammingLanguage).NotNull().NotEmpty();
            RuleFor(c => c.LanguageLevel).NotNull().NotEmpty();
            RuleFor(c => c.Country).NotNull().NotEmpty();
            RuleFor(c => c.City).NotNull().NotEmpty();
            RuleFor(c => c.Skills).NotNull().NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, Result<ResumeResponse>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Result<ResumeResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _appDbContext.Users.FindAsync(request.UserId);

            if (user is null)
                return Result.Fail<ResumeResponse>("Пользователь не найден.");

            var resume = new Resume
            {
                Status = Status.Pending,
                IsRejected = true,
                ProgrammingLanguage = request.ProgrammingLanguage,
                FullName = request.FullName,
                LanguageLevel = request.LanguageLevel,
                Country = request.Country,
                City = request.City,
                Skills = request.Skills,
                Links = request.Links,
                EducationPeriods = request.EducationPeriods.Adapt<List<EducationPeriod>>(),
                WorkPeriods = request.WorkPeriods.Adapt<List<WorkPeriod>>(),
            };

            user.Resumes.Add(resume);

            await _appDbContext.SaveChangesAsync();

            var response = resume.Adapt<ResumeResponse>();

            return response;
        }
    }
}
