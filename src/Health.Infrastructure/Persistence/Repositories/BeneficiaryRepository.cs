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
}