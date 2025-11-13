using Health.Domain.Entities;
using Health.Domain.Repositories;

namespace Health.Infrastructure.Persistence.Repositories;

public sealed class HealthPlanRepository(AppDbContext context)
    : BaseRepository<HealthPlan>(context), IHealthPlanRepository;