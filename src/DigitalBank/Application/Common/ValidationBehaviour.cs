using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace DigitalBank.Application.Common;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var error = new Error("Validation.Failed", string.Join("; ", failures.Select(f => f.ErrorMessage)));

            // Handle both Result and Result<T> cases
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var genericArgument = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
                    .MakeGenericMethod(genericArgument);
                return (TResponse)failureMethod.Invoke(null, [error])!;
            }

            return (TResponse)Result.Failure(error);
        }

        return await next(cancellationToken);
    }
}