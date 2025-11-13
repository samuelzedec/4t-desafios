using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Health.Infrastructure.Persistence.Factories;

/// <summary>
/// Uma fábrica de classe que implementa <see cref="IDesignTimeDbContextFactory&lt;TContext&gt;"/> para criar instâncias de <see cref="AppDbContext"/>
/// durante operações em tempo de design como migrações.
/// </summary>
/// <remarks>
/// Esta fábrica é utilizada principalmente pelas ferramentas CLI do Entity Framework para inicializar o <see cref="AppDbContext"/>
/// com a configuração apropriada durante migrações ou outras tarefas em tempo de design.
/// Ela utiliza o arquivo `appsettings.json` para recuperar valores de configuração, incluindo strings de conexão do banco de dados.
/// </remarks>
/// <seealso cref="AppDbContext"/>
/// <seealso cref="IDesignTimeDbContextFactory&lt;TContext&gt;"/>
public sealed class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string AppSettingsPath = "../../../../Health.Api/appsettings.json";

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, AppSettingsPath), optional: false)
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(
                configuration.GetConnectionString("PostgresConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                    .MigrationsHistoryTable("__EFMigrationsHistory"))
            .EnableDetailedErrors()
            .EnableServiceProviderCaching();

        return new AppDbContext(dbContextOptionsBuilder.Options);
    }
}