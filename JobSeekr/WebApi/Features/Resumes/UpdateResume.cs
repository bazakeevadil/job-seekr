using WebApi.Shared.Helpers;

namespace WebApi.Features.Resumes;

public static class UpdateResume
{
    public record Command : IRequest<Result>
    {
        public required string Email { get; init; }
        public required string Password { get; init; }

        public required PatchResumeProps Props { get; init; }
    }

    public record PatchResumeProps
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null || !AuthHelper.CheckPassword(user, request.Password))
                return Result.Bad<string>("Пользователь не найден или пароль указан неверно.");



            _context.Update(user);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
