using Health.Domain.Entities;

namespace Health.Domain.Repositories;

/// <summary>
/// Define uma interface genérica de repositório para gerenciar entidades do tipo <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// O tipo da entidade que este repositório gerencia. Deve herdar de <see cref="BaseEntity"/>.
/// </typeparam>
public interface IRepository<in T>
    where T : BaseEntity
{
    /// <summary>
    /// Cria uma nova entidade do tipo <typeparamref name="T"/> de forma assíncrona e adiciona ao repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser criada.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para monitorar requisições de cancelamento.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona.
    /// </returns>
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente do tipo <typeparamref name="T"/> no repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser atualizada.
    /// </param>
    void Update(T entity);

    /// <summary>
    /// Remove uma entidade existente do tipo <typeparamref name="T"/> do repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser removida.
    /// </param>
    void Delete(T entity);
}