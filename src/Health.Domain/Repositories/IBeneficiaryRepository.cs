using Health.Domain.Entities;

namespace Health.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório para gerenciar e realizar operações CRUD
/// na entidade <see cref="Beneficiary"/> no domínio.
/// </summary>
public interface IBeneficiaryRepository : IRepository<Beneficiary>;