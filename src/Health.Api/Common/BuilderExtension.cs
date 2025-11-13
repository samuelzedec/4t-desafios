using Health.Application;
using Health.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

namespace Health.Api.Common;

internal static class BuilderExtension
{
    internal static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.AddConfigurations();
        builder.AddDependencyInjection();
        builder.AddDocumentationApi();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, builder.Logging);
    }

    private static void AddDocumentationApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options => options
            .AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Health API",
                    Version = "v1",
                    Description = "Api para cadastro de planos de saúde e beneficiários"
                };
                return Task.CompletedTask;
            })
        );
    }

    private static void AddConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Configuration.AddEnvironmentVariables();
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
            options.ConfigureEndpointDefaults(endpoint
                => endpoint.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);
        });
    }
}