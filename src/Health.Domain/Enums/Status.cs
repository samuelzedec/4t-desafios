using System.ComponentModel;

namespace Health.Domain.Enums;

/// <summary>
/// Representa o estado operacional de uma entidade.
/// </summary>
public enum Status
{
    [Description("Ativo")]
    Active = 1,
    [Description("Inativo")]
    Inactive = 2
}