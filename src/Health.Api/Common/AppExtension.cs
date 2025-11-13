using Scalar.AspNetCore;

namespace Health.Api.Common;

internal static class AppExtension
{
    internal static void UseApiServices(this WebApplication app)
    {
        app.UseDocumentation();
    }

    private static void UseDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    private static void UseConfigurations(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            await next();
        });

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
    }
}