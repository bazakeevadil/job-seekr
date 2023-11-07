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
        public string? FullName { get; init; }
        public string? ProgrammingLanguage { get; init; }
        public string? LanguageLevel { get; init; }
        public string? Country { get; init; }
        public string? City { get; init; }
        public string? Links { get; init; }
        public string? Skills { get; init; }

        public List<EducationPeriodProps> EducationPeriods { get; init; } = new();
        public List<WorkPeriodProps> WorkPeriods { get; init; } = new();

    }

    public record EducationPeriodProps
    {
        public long Id { get; init; }
        public long ResumeId { get; init; }
        public string? Name { get; init; }
        public string? Degree { get; init; }
        public string? City { get; init; }
        public string? Description { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
    }

    public record WorkPeriodProps
    {
        public long Id { get; init; }
        public long ResumeId { get; init; }
        public string? Position { get; init; }
        public string? Employer { get; init; }
        public string? City { get; init; }
        public string? Description { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Props).NotNull();

            When(c => c.Props is not null, () =>
            {
                RuleFor(c => c.Props.FullName).NotEmpty();
                RuleFor(c => c.Props.ProgrammingLanguage).NotEmpty();
                RuleFor(c => c.Props.LanguageLevel).NotEmpty();
                RuleFor(c => c.Props.Country).NotEmpty();
                RuleFor(c => c.Props.City).NotEmpty();
                RuleFor(c => c.Props.Skills).NotEmpty();

                RuleForEach(c => c.Props.EducationPeriods)
                .ChildRules(period =>
                {
                    period.RuleFor(p => p.City).NotEmpty();
                    period.RuleFor(p => p.Name).NotEmpty();
                    period.RuleFor(p => p.Degree).NotEmpty();
                    period.RuleFor(p => p.Description).NotEmpty();
                    period.RuleFor(p => p.From).NotEmpty();
                    period.RuleFor(p => p.To).NotEmpty();
                });

                RuleForEach(c => c.Props.WorkPeriods)
                .ChildRules(period =>
                {
                    period.RuleFor(p => p.City).NotEmpty();
                    period.RuleFor(p => p.Position).NotEmpty();
                    period.RuleFor(p => p.Employer).NotEmpty();
                    period.RuleFor(p => p.Description).NotEmpty();
                    period.RuleFor(p => p.From).NotEmpty();
                    period.RuleFor(p => p.To).NotEmpty();
                });
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
            var resume = await _context.Resumes.FirstOrDefaultAsync(r =>
                r.Id == request.Id
                && r.UserId == request.UserId);

            if (resume is null)
                return Result.Fail("Резюме не найдено.");

            if (!string.IsNullOrEmpty(request.Props.FullName))
                resume.FullName = request.Props.FullName;

            if (!string.IsNullOrEmpty(request.Props.ProgrammingLanguage))
                resume.ProgrammingLanguage = request.Props.ProgrammingLanguage;

            if (!string.IsNullOrEmpty(request.Props.LanguageLevel))
                resume.LanguageLevel = request.Props.LanguageLevel;

            if (!string.IsNullOrEmpty(request.Props.Country))
                resume.Country = request.Props.Country;

            if (!string.IsNullOrEmpty(request.Props.City))
                resume.City = request.Props.City;

            if (!string.IsNullOrEmpty(request.Props.Links))
                resume.Links = request.Props.Links;

            if (!string.IsNullOrEmpty(request.Props.Skills))
                resume.Skills = request.Props.Skills;

            foreach (var period in request.Props.EducationPeriods
                .Where(p => resume.EducationPeriods
                    .Any(x => x.Id == p.Id)))
            {
                var oldPeriod = resume.EducationPeriods.First(x => x.Id == period.Id);
                if(!string.IsNullOrEmpty(period.Name))
                    oldPeriod.Name = period.Name;

                if (!string.IsNullOrEmpty(period.Degree))
                    oldPeriod.Degree = period.Degree;

                if (!string.IsNullOrEmpty(period.Description))
                    oldPeriod.Description = period.Description;

                if (!string.IsNullOrEmpty(period.City))
                    oldPeriod.City = period.City;

                if (period.From is null)
                    oldPeriod.From = period.From;

                if (period.To is null)
                    oldPeriod.To = period.To;
            }

            foreach (var period in request.Props.WorkPeriods
                .Where(p => resume.WorkPeriods
                    .Any(x => x.Id == p.Id)))
            {
                var oldPeriod = resume.WorkPeriods.First(x => x.Id == period.Id);
                if (!string.IsNullOrEmpty(period.Employer))
                    oldPeriod.Employer = period.Employer;

                if (!string.IsNullOrEmpty(period.Description))
                    oldPeriod.Description = period.Description;

                if (!string.IsNullOrEmpty(period.Position))
                    oldPeriod.Position = period.Position;

                if (period.From is null)
                    oldPeriod.From = period.From;

                if (period.To is null)
                    oldPeriod.To = period.To;

                if (!string.IsNullOrEmpty(period.City))
                    oldPeriod.City = period.City;
            }

            _context.Update(resume);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
