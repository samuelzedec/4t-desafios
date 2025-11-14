using FluentValidation;
using Health.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Health.Application;

/// <summary>
/// Fornece métodos de extensão para configurar a injeção de dependência
/// relacionada à camada de aplicação.
/// </summary>
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true
        );
    }
}