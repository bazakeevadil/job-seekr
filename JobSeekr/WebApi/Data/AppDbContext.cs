using WebApi.Domain.Entities;

namespace WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<WorkPeriod> WorkPeriods => Set<WorkPeriod>();
    public DbSet<EducationPeriod> EducationPeriods => Set<EducationPeriod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
    }
}
