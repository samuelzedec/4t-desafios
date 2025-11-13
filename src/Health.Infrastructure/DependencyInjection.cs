using Health.Domain.Repositories;
using Health.Infrastructure.Persistence;
using Health.Infrastructure.Persistence.Interceptors;
using Health.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Health.Infrastructure;

/// <summary>
/// Fornece métodos de extensão para configurar os serviços de infraestrutura,
/// incluindo configuração de persistência e injeção de dependência para a aplicação.
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder loggingBuilder)
    {
        var connectionString = configuration.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");

        services.AddPersistence(connectionString);
        services.AddLogger(loggingBuilder);
    }

    private static void AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(connectionString, n => n
                .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                .MigrationsHistoryTable("__EFMigrationsHistory"))
            .AddInterceptors(new AuditInterceptor(), new CaseInterceptor())
            .EnableDetailedErrors()
            .EnableServiceProviderCaching()
        );
    }

    private static void AddLogger(this IServiceCollection _, ILoggingBuilder loggingBuilder)
    {
        const string logStructure = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: logStructure)
            .CreateLogger();

        loggingBuilder.AddSerilog();
    }
    
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBeneficiaryRepository, BeneficiaryRepository>();
        services.AddTransient<IHealthPlanRepository, HealthPlanRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}