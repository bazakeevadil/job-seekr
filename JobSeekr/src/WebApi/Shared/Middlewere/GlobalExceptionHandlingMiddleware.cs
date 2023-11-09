using System.Net;
using System.Text.Json;

namespace WebApi.Shared.Middlewere;

/// <summary>
/// Middleware для глобальной обработки исключений.
/// </summary>
public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GlobalExceptionHandlingMiddleware"/> класса.
    /// </summary>
    /// <param name="logger">Регистратор.</param>
    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Асинхронно вызывает промежуточное программное обеспечение.
    /// </summary>
    /// <param name="context">HTTP-контекст.</param>
    /// <param name="next">Делегат обработчика запроса.</param>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var problem = Result.Fail(ex.Message);

            string json = JsonSerializer.Serialize(problem);

            await context.Response.WriteAsync(json);
        }
    }
}
