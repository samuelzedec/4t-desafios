namespace Health.Domain.Entities;

/// <summary>
/// Representa a classe base abstrata para todas as entidades no domínio.
/// </summary>
public abstract class BaseEntity
{
    #region Properties

    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    #endregion

    #region Methods

    public void UpdateEntity()
        => UpdatedAt = DateTime.UtcNow;

    public void DeleteEntity()
        => DeletedAt = DateTime.UtcNow;

    #endregion

    #region Overrides

    public override int GetHashCode()
        => Id.GetHashCode();

    #endregion
}