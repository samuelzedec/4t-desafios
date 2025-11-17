using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Health.Application;
using Health.Application.Common;
using Health.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using EmptyResult = Microsoft.AspNetCore.Mvc.EmptyResult;

namespace Health.Api.Common;

internal static class BuilderExtension
{
    internal static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.AddConfigurations();
        builder.AddControllerConfiguration();
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
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.WebHost.ConfigureKestrel(options =>
            options.AddServerHeader = false);

        builder.Services.Configure<JsonOptions>(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    }

    private static void AddControllerConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key.Replace("$.", ""),
                        kvp => kvp.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    );

                var result = Result.Failure<EmptyResult>(
                    "Dados de entrada inválidos.",
                    HttpStatusCode.BadRequest,
                    errors
                );

                return new BadRequestObjectResult(result);
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            });
    }
}