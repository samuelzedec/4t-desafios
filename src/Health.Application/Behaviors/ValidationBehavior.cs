using FluentValidation;
using Health.Application.Exceptions;
using MediatR;

namespace Health.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var errors = validators
            .Select(v => v.Validate(context))
            .Where(v => v.Errors.Count > 0)
            .SelectMany(v => v.Errors)
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => m.ErrorMessage).ToArray()
            );

        return errors.Count > 0
            ? throw new BadRequestException("Dados Inválidos", errors)
            : await next(cancellationToken);
    }
}