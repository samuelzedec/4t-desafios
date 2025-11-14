using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Health.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Processing {RequestName}...", requestName);
        var response = await next(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "{RequestName} completed in {ElapsedMs}ms",
            requestName,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}