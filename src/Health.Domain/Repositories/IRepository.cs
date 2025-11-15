using System.Linq.Expressions;
using Health.Domain.Entities;

namespace Health.Domain.Repositories;

/// <summary>
/// Define uma interface genérica de repositório para gerenciar entidades do tipo <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// O tipo da entidade que este repositório gerencia. Deve herdar de <see cref="BaseEntity"/>.
/// </typeparam>
public interface IRepository<T>
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

    /// <summary>
    /// Verifica de forma assíncrona se existe uma entidade do tipo <typeparamref name="T"/> que satisfaz o predicado especificado.
    /// </summary>
    /// <param name="predicate">
    /// Expressão utilizada para definir a condição a ser verificada.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para monitorar requisições de cancelamento.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona e retorna um valor booleano indicando se existe alguma entidade que satisfaça a condição.
    /// </returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}