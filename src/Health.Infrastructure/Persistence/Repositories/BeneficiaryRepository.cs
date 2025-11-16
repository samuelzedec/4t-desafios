using Health.Domain.Entities;
using Health.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Health.Infrastructure.Persistence.Repositories;

public sealed class BeneficiaryRepository(AppDbContext context)
    : BaseRepository<Beneficiary>(context), IBeneficiaryRepository
{
    public async Task<Beneficiary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _table
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<Beneficiary?> GetByIdWithHealthPlanAsync(Guid id, CancellationToken cancellationToken = default)
        => await _table
            .AsNoTracking()
            .Include(b => b.HealthPlan)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
}