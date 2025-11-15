using Health.Domain.Entities;

namespace Health.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório para gerenciar entidades de planos de saúde,
/// fornecendo uma abstração para operações CRUD específicas para <see cref="HealthPlan"/>.
/// </summary>
public interface IHealthPlanRepository : IRepository<HealthPlan>
{
    /// <summary>
    /// Recupera uma entidade de plano de saúde pelo seu identificador único.
    /// </summary>
    /// <param name="id">
    /// O identificador único do plano de saúde a ser recuperado.
    /// </param>
    /// <param name="cancellationToken">
    /// Um token para monitorar solicitações de cancelamento. O padrão é <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a entidade do plano de saúde
    /// se encontrada, ou <c>null</c> se nenhum plano de saúde existir com o identificador especificado.
    /// </returns>
    Task<HealthPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera uma entidade de plano de saúde pelo seu identificador único,
    /// incluindo os beneficiários associados ao plano.
    /// </summary>
    /// <param name="id">
    /// O identificador único do plano de saúde a ser recuperado.
    /// </param>
    /// <param name="cancellationToken">
    /// Um token para monitorar solicitações de cancelamento. O padrão é <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a entidade do plano de saúde,
    /// incluindo os beneficiários associados, se encontrado, ou <c>null</c> se nenhum plano de saúde existir com
    /// o identificador especificado.
    /// </returns>
    Task<HealthPlan?> GetByIdWithBeneficiariesAsync(Guid id, CancellationToken cancellationToken = default);
}