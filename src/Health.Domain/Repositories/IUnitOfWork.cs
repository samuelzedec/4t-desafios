namespace Health.Domain.Repositories;

/// <summary>
/// Define uma interface para a unidade de trabalho (Unit of Work), que gerencia e coordena
/// operações que envolvem múltiplos repositórios e persistência de dados.
/// </summary>
public interface IUnitOfWork
{
    #region Properties

    IBeneficiaryRepository Beneficiaries { get; }
    IHealthPlanRepository HealthPlans { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Salva todas as mudanças feitas no contexto para o banco de dados de forma assíncrona
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o número de registros gravados no banco de dados.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma nova transação de banco de dados de forma assíncrona.
    /// Esta transação encapsula uma série de operações que podem ser confirmadas ou revertidas como uma unidade.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação atual de forma assíncrona, persistindo todas as alterações realizadas.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverte todas as operações realizadas na transação atual de forma assíncrona.
    /// Esta operação é usada para cancelar todas as mudanças não confirmadas no contexto persistente.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    #endregion
}