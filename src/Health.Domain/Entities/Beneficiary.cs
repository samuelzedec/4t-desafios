using Health.Domain.Enums;
using Health.Domain.ValueObjects.BirthDate;
using Health.Domain.ValueObjects.Cpf;
using Health.Domain.ValueObjects.Name;

namespace Health.Domain.Entities;

public sealed class Beneficiary : BaseEntity
{
    #region Properties

    public Name FullName { get; private set; }
    public Cpf Cpf { get; private set; }
    public Status Status { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public Guid HealthPlanId { get; private set; }

    #endregion

    #region Nagivation Properties

    public HealthPlan HealthPlan { get; private set; } = null!;

    #endregion

    #region Constructors

#pragma warning disable CS8618
    private Beneficiary() { }
#pragma warning restore CS8618

    private Beneficiary(Name fullName, Cpf cpf, BirthDate birthDate, Guid healthPlanId)
    {
        FullName = fullName;
        Cpf = cpf;
        Status = Status.Active;
        BirthDate = birthDate;
        HealthPlanId = healthPlanId;
    }

    #endregion

    #region Factories Methods

    public static Beneficiary Create(string fullName, string cpf, DateOnly birthDate, Guid flatId)
        => new(Name.Create(fullName), Cpf.Create(cpf), BirthDate.Create(birthDate), flatId);

    #endregion

    #region Methods

    public void UpdateStatus(Status status)
        => Status = status;

    public void UpdateFlatId(Guid healthPlanId)
        => HealthPlanId = healthPlanId;

    public void UpdateFullName(string fullName)
        => FullName = Name.Create(fullName);

    public void UpdateCpf(string cpf)
        => Cpf = Cpf.Create(cpf);

    public void UpdateBirthDate(DateOnly birthDate)
        => BirthDate = BirthDate.Create(birthDate);

    #endregion
}