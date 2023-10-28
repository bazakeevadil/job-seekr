using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("Default")));

        return services;
    }
}
