using Health.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Health.Infrastructure;

/// <summary>
/// Fornece métodos de extensão para configurar os serviços de infraestrutura,
/// incluindo configuração de persistência e injeção de dependência para a aplicação.
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddPersistence(connectionString);
    }

    private static void AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(connectionString, n => n
                .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                .MigrationsHistoryTable("__EFMigrationsHistory"))
            .EnableDetailedErrors()
            .EnableServiceProviderCaching()
        );
    }
}