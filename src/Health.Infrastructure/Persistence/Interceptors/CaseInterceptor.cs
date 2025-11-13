using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Health.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Um interceptador de comandos do Entity Framework Core que modifica consultas SQL para realizar
/// comparações de strings sem diferenciação de maiúsculas e minúsculas em comandos contendo o operador "LIKE"
/// substituindo-o por "ILIKE".
/// </summary>
/// <remarks>
/// Este interceptador foi projetado para ajustar o comportamento de consultas de banco de dados em ambientes
/// onde é necessária correspondência de strings sem diferenciação de maiúsculas e minúsculas para operações "LIKE".
/// Ele escuta eventos de execução de comandos e atualiza o texto do comando SQL de acordo.
/// </remarks>
/// <seealso cref="Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor" />
public sealed class CaseInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        MakeLikeCaseInsensitive(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        MakeLikeCaseInsensitive(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    private static void MakeLikeCaseInsensitive(DbCommand command)
    {
        if (command.CommandText.Contains("LIKE"))
            command.CommandText = command.CommandText.Replace("LIKE", "ILIKE");
    }
}