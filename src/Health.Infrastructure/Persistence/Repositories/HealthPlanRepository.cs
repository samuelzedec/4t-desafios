using Health.Domain.Entities;
using Health.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Health.Infrastructure.Persistence.Repositories;

public sealed class HealthPlanRepository(AppDbContext context)
    : BaseRepository<HealthPlan>(context), IHealthPlanRepository
{
    public async Task<HealthPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _table
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public async Task<HealthPlan?> GetByIdWithBeneficiariesAsync(Guid id, CancellationToken cancellationToken = default)
        => await _table
            .Include(h => h.Beneficiaries)
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
}