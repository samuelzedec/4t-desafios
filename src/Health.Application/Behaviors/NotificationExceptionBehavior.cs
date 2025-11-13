using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Health.Application.Behaviors;

/// <summary>
/// Ação de exceção para o pipeline do MediatR que registra erros sem resolvê-los.
/// A exceção é relançada após a execução.
/// <br/>
/// Fonte: <see href="https://github.com/LuckyPennySoftware/MediatR/wiki#exception-action-pipeline-step">MediatR Exception Action Pipeline Step</see>
/// </summary>
/// <typeparam name="TRequest">
/// O tipo da requisição. Deve implementar <see cref="IBaseRequest"/>.
/// </typeparam>
/// <remarks>
/// Executa logging e notificações quando uma exceção ocorre, mas não a resolve.
/// </remarks>
public sealed class NotificationExceptionBehavior<TRequest>(
    ILogger<NotificationExceptionBehavior<TRequest>> logger)
    : IRequestExceptionAction<TRequest, Exception> where TRequest : class, IBaseRequest
{
    public Task Execute(
        TRequest request,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Request {RequestName} failed with exception {ExceptionType}: {ExceptionMessage}",
            request.GetType().Name,
            exception.GetType().Name,
            exception.Message
        );

        return Task.CompletedTask;
    }
}