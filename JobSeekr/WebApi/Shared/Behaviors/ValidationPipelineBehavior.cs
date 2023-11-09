using FluentValidation;

namespace WebApi.Shared.Behaviors;

/// <summary>
/// Поведение конвейера валидации запросов.
/// </summary>
/// <typeparam name="TRequest">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип ответа.</typeparam>
public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IValidator<TRequest> _validator;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ValidationPipelineBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="validator">Валидатор запросов.</param>
    public ValidationPipelineBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// Обрабатывает запрос.
    /// </summary>
    /// <param name="request">Запрос для обработки.</param>
    /// <param name="next">Делегат для вызова следующего обработчика в цепочке.</param>
    /// <param name="cancellationToken">Токен отмены для отмены операции.</param>
    /// <returns>Результат обработки запроса.</returns>
    /// <exception cref="Exception">Результат проверки не является поддерживаемым типом.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var valResult = _validator.Validate(request);

        if (valResult != null && valResult.Errors.Any())
        {
            var errors = valResult.Errors.Select(f => new Error(f.ErrorMessage, f.ErrorCode)).ToList();

            if (typeof(TResponse) == typeof(Result))
            {
                return (Result.Fail(errors) as TResponse)!;
            }

            var response = Activator.CreateInstance(typeof(TResponse), errors);

            if (response is not TResponse result)
                throw new Exception("Результат проверки не является поддерживаемым типом.");

            return result;
        }

        return await next();
    }
}

