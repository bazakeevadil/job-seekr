namespace WebApi.Data;

/// <summary>
/// Контекст базы данных приложения.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AppDbContext"/>.
    /// </summary>
    /// <param name="options">Параметры контекста базы данных.</param>
    public AppDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Набор данных пользователей.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>\
    /// Набор данных резюме.
    /// </summary>
    public DbSet<Resume> Resumes => Set<Resume>();

    /// <summary>
    /// Набор данных периодов работы.
    /// </summary>
    public DbSet<WorkPeriod> WorkPeriods => Set<WorkPeriod>();

    /// <summary>
    /// Набор данных периодов образования.
    /// </summary>
    public DbSet<EducationPeriod> EducationPeriods => Set<EducationPeriod>();

    /// <summary>
    /// Переопределение метода OnModelCreating для настройки модели базы данных.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели базы данных.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin$228");

        // Начальные данные для таблицы пользователей
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

        // Включение автоматической загрузки связанных данных
        modelBuilder.Entity<User>().Navigation(u => u.Resumes).AutoInclude();
        modelBuilder.Entity<Resume>().Navigation(r => r.WorkPeriods).AutoInclude();
        modelBuilder.Entity<Resume>().Navigation(r => r.EducationPeriods).AutoInclude();

        modelBuilder.Entity<ResumePhoto>().ToTable("ResumePhotos");
        modelBuilder.Entity<ResumePhoto>().HasKey(r => r.ResumeId);

        var user = modelBuilder.Entity<User>();
        // Уникальный индекс для поля Email
        user.HasIndex(u => u.Email).IsUnique();
        // Использование заданной коллации для поля Email
        user.Property(u => u.Email).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
