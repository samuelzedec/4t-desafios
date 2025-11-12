using Health.Domain.ValueObjects.AnsRegistrationCode;
using Health.Domain.ValueObjects.Name;

namespace Health.Domain.Entities;

public sealed class HealthPlan : BaseEntity
{
    #region Properties

    public Name Name { get; private set; }
    public AnsRegistrationCode AnsRegistrationCode { get; private set; }

    #endregion

    #region Navigation Properties

    public IReadOnlyCollection<Beneficiary> Beneficiaries { get; private set; } = [];

    #endregion

    #region Constructors

#pragma warning disable CS8618
    private HealthPlan() { }
#pragma warning restore CS8618

    private HealthPlan(Name name, AnsRegistrationCode ansRegistrationCode)
    {
        Name = name;
        AnsRegistrationCode = ansRegistrationCode;
    }

    #endregion

    #region Factories

    public static HealthPlan Create(string name, string ansRegistrationCode)
        => new(Name.Create(name), AnsRegistrationCode.Create(ansRegistrationCode));

    #endregion

    #region Methods

    public void UpdateName(string name)
        => Name = Name.Create(name);

    public void UpdateAnsCode(string ansCode)
        => AnsRegistrationCode = AnsRegistrationCode.Create(ansCode);
    
    #endregion
}