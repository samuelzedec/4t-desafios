using System.Net;

namespace Health.Application.Exceptions;

public sealed class BadRequestException(string message, Dictionary<string, string[]>? details = null)
    : ApplicationLayerException(message, HttpStatusCode.BadRequest, details);