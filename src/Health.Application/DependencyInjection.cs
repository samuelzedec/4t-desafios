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
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true
        );
    }
}