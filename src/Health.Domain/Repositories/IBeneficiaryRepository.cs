using Health.Domain.Entities;

namespace Health.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório para gerenciar e realizar operações CRUD
/// na entidade <see cref="Beneficiary"/> no domínio.
/// </summary>
public interface IBeneficiaryRepository : IRepository<Beneficiary>
{
    /// <summary>
    /// Busca um beneficiário pelo identificador único (ID) informado.
    /// </summary>
    /// <param name="id">O ID do beneficiário a ser pesquisado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>
    /// Retorna o beneficiário correspondente ao ID, ou null caso não seja encontrado.
    /// </returns>
    Task<Beneficiary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um beneficiário pelo ID com os dados do plano de saúde carregados.
    /// </summary>
    /// <param name="id">O identificador único do beneficiário.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>
    /// O beneficiário com o plano de saúde incluído, ou null se não encontrado.
    /// </returns>
    Task<Beneficiary?> GetByIdWithHealthPlanAsync(Guid id, CancellationToken cancellationToken = default);
}