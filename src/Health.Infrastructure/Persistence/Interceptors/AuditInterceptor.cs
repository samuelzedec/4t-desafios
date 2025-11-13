using Health.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Health.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Intercepta e customiza o comportamento dos métodos SaveChanges e SaveChangesAsync no Entity Framework.
/// Utilizado para aplicar operações de auditoria em entidades que derivam de <see cref="BaseEntity"/>.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            TrackAuditChanges(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            TrackAuditChanges(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void TrackAuditChanges(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Modified)
                entry.Entity.UpdateEntity();

            if (entry.State is not EntityState.Deleted)
                continue;

            entry.State = EntityState.Modified;
            entry.Entity.DeleteEntity();
        }
    }
}