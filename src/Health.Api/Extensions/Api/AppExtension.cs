using Scalar.AspNetCore;

namespace Health.Api.Extensions.Api;

internal static class AppExtension
{
    internal static void UseApiServices(this WebApplication app)
    {
        app.ConfigureDocumentation();
    }

    private static void ConfigureDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}