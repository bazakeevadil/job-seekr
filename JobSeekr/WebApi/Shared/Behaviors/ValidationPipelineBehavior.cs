using FluentValidation;

namespace WebApi.Shared.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IValidator<TRequest> _validator;

    public ValidationPipelineBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var valResult = _validator.Validate(request);

        if (valResult != null && valResult.Errors.Any())
        {
            var errors = valResult.Errors.Select(f => new Error(f.ErrorMessage, f.ErrorCode)).ToList();

            var response = Activator.CreateInstance(typeof(TResponse), errors);

            if (response is not TResponse result)
                throw new Exception("Результат проверки не является поддерживаемым типом.");

            return result;
        }

        return await next();
    }
}

