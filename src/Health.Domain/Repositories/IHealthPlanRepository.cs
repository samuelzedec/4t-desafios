using Health.Domain.Entities;

namespace Health.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório para gerenciar entidades de planos de saúde,
/// fornecendo uma abstração para operações CRUD específicas para <see cref="HealthPlan"/>.
/// </summary>
public interface IHealthPlanRepository : IRepository<HealthPlan>;