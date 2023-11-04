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
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin");
        modelBuilder.Entity<User>()
            .HasData(
                new User
                {
                    Id = 228,
                    Email = "admin@gmail.com",
                    HashPassword = passwordHash,
                    IsBlocked = false,
                    Role = Role.Admin,
                });

        modelBuilder.Entity<User>()
            .Navigation(u => u.Resumes).AutoInclude();
    }
}
