using Mapster;
using WebApi.Contract.Request;
using WebApi.Contract.Response;
using WebApi.Features.Auth;

namespace WebApi.Features.Resumes;

public static class AddResume
{
    public record Command : IRequest<Result<ResumeResponse>>
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

    internal class Handler : IRequestHandler<Command, Result<ResumeResponse>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Result<ResumeResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var resume = new Resume
            {
                Status = Status.Pending,
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

            _appDbContext.Resumes.Add(resume);
            await _appDbContext.SaveChangesAsync();

            var response = resume.Adapt<ResumeResponse>();

            return response;
        }
    }
}

public class AddResumeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/resume/add",
            async (IMediator mediator, AddResumeRequest request) =>
            {
                var command = request.Adapt<AddResume.Command>();

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result.Value);
            })
            .WithSummary("Создание резюме")
            .WithDescription("Создать резюме текушего пользователя")
            .Produces<Result>(400)
            .WithOpenApi();
    }
}
