using Health.Domain.Entities;

namespace Health.Domain.Abstractions;

public interface IFilter<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query);
}