using System.Net;

namespace Health.Application.Exceptions;

public abstract class ApplicationLayerException(
    string message,
    HttpStatusCode statusCode,
    Dictionary<string, string[]>? details = null)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; init; } = statusCode;
    public Dictionary<string, string[]>? Details { get; init; } = details;
}