using Health.Domain.Entities;
using Health.Domain.Repositories;

namespace Health.Infrastructure.Persistence.Repositories;

public sealed class BeneficiaryRepository(AppDbContext context)
    : BaseRepository<Beneficiary>(context), IBeneficiaryRepository;