using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Health.Application.Abstractions.Common;
using Health.Application.Exceptions;
using Health.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Health.Api.Common;

internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {ExceptionType} - {Message}",
            exception.GetType().Name, exception.Message);

        (string message, HttpStatusCode code, Dictionary<string, string[]>? details) = MapExceptionToError(exception);

        var jsonResponse = JsonSerializer.Serialize(
            Result.Failure<EmptyResult>(message, code, details),
            JsonSerializerOptions
        );

        httpContext.Response.StatusCode = (int)code;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);
        return true;
    }

    private static (string, HttpStatusCode, Dictionary<string, string[]>?) MapExceptionToError(Exception exception)
        => exception switch
        {
            ApplicationLayerException appEx => (appEx.Message, appEx.StatusCode, appEx.Details),
            DomainException domainEx => (domainEx.Message, HttpStatusCode.UnprocessableContent, null),
            _ => ("Erro inesperado no servidor.", HttpStatusCode.InternalServerError, null)
        };
}