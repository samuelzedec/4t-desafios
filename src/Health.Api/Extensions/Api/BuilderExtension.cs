using Health.Application;
using Health.Infrastructure;
using Microsoft.OpenApi.Models;

namespace Health.Api.Extensions.Api;

internal static class BuilderExtension
{
    internal static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.AddDependencyInjection();
        builder.AddDocumentationApi();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
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
}