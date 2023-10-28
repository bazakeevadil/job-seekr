using Microsoft.EntityFrameworkCore;
using System.Reflection;
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

    public static IServiceCollection AddMediatr(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(
                Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
