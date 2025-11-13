using Health.Domain.Entities;
using Health.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Health.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<T>(AppDbContext context)
    : IRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> _table = context.Set<T>();

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        => await _table.AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _table.Update(entity);

    public void Delete(T entity)
        => _table.Remove(entity);
}