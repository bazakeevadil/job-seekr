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
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin$228");
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

        modelBuilder.Entity<User>().Navigation(u => u.Resumes).AutoInclude();
        modelBuilder.Entity<Resume>().Navigation(r => r.WorkPeriods).AutoInclude();
        modelBuilder.Entity<Resume>().Navigation(r => r.EducationPeriods).AutoInclude();

        var user = modelBuilder.Entity<User>();
        user.HasIndex(u => u.Email).IsUnique();
        user.Property(u => u.Email).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
