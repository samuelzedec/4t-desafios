using Health.Domain.Entities;

namespace Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;

public sealed record CreateBeneficiaryCommandResponse(
    Guid Id,
    string FullName,
    string Cpf,
    string BirthDate,
    string HealthPlanName,
    string Status)
{
    public static CreateBeneficiaryCommandResponse Create(
        Beneficiary beneficiary,
        HealthPlan healthPlan)
        => new(
            beneficiary.Id,
            beneficiary.FullName,
            beneficiary.Cpf.Value,
            beneficiary.BirthDate,
            healthPlan.Name,
            beneficiary.Status.ToString()
        );
}