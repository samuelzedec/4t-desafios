using Health.Application.DTOs;
using Health.Application.Extensions;
using Health.Domain.Entities;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaryByIdQuery;

public sealed record GetBeneficiaryByIdQueryResponse(
    Guid Id,
    string FullName,
    string Cpf,
    string BirthDate,
    string Status,
    HealthPlanMinimalDto HealthPlan,
    DateTime CreationDate,
    DateTime? ModificationDate)
{
    public static GetBeneficiaryByIdQueryResponse FromEntity(Beneficiary beneficiary)
        => new(
            beneficiary.Id,
            beneficiary.FullName,
            beneficiary.Cpf,
            beneficiary.BirthDate,
            beneficiary.Status.GetDescription(),
            new HealthPlanMinimalDto(beneficiary.HealthPlan.Name, beneficiary.HealthPlan.AnsRegistrationCode),
            beneficiary.CreatedAt,
            beneficiary.UpdatedAt
        );
}