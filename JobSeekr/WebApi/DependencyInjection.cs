using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace WebApi;

/// <summary>
/// Класс, предоставляющий статические методы для регистрации зависимостей.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует контекст базы данных в контейнере служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Коллекция служб с зарегистрированным контекстом базы данных.</returns>
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("Default")));

        return services;
    }

    /// <summary>
    /// Регистрирует MediatR в контейнере служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <returns>Коллекция служб с зарегистрированным MediatR.</returns>
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

    /// <summary>
    /// Регистрирует авторизацию в контейнере служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Коллекция служб с зарегистрированной авторизацией.</returns>
    /// <exception cref="Exception">Секретный ключ для генерации JWT не найден в файле кофигурации.</exception>
    public static IServiceCollection AddAuth(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.SaveToken = true;
            opts.RequireHttpsMetadata = false;
            opts.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            configuration["JWT_TOKEN"] ??
                                throw new Exception("Секретный ключ для генерации JWT не найден в файле кофигурации."))),
            };
        });

        services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            opts.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        });

        return services;
    }

    /// <summary>
    /// Регистрирует Swagger в контейнере служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <returns>Коллекция служб с зарегистрированным Swagger.</returns>
    public static IServiceCollection AddSwagger(
       this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "JobSeekrApi",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Регистрирует CORS политику в контейнере служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <returns>Коллекция служб с зарегистрированным CORS.</returns>
    public static IServiceCollection AddAnyCors(
       this IServiceCollection services)
    {
        services.AddCors(o => o.AddDefaultPolicy(
        builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

        return services;
    }
}
