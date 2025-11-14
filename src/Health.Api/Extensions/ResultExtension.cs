using System.Net;
using Health.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Health.Api.Extensions;

public static class ResultExtension
{
    public static IActionResult ToHttpResponse<T>(this Result<T> result, string url = "")
        => result.StatusCode is HttpStatusCode.Created
            ? new CreatedResult(url, result)
            : new ObjectResult(result) { StatusCode = (int)result.StatusCode };
}