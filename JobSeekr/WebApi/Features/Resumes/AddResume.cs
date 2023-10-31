using Mapster;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;

namespace WebApi.Features.Resumes;

public static class AddResume
{
    public record Command : IRequest<Result<ResumeDto>>
    {
        public required string FullName { get; init; }
        public required string ProgrammingLanguage { get; init; }
        public required string LanguageLevel { get; init; }
        public required string Country { get; init; }
        public required string City { get; init; }
        public string? Links { get; init; }
        public required string Skills { get; init; }

        public List<EducationPeriodDto> EducationPeriods { get; init; } = new();
        public List<WorkPeriodDto> WorkPeriods { get; init; } = new();
    }

    internal class Handler : IRequestHandler<Command, Result<ResumeDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Result<ResumeDto>> Handle(Command request, CancellationToken cancellationToken)
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

            var response = resume.Adapt<ResumeDto>();

            return response;
        }
    }
}
