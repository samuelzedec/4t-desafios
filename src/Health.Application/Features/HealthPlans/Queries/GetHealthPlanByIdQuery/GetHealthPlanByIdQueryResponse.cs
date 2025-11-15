using Health.Application.Abstractions.Queries;
using Health.Application.DTOs;
using Health.Domain.Entities;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlanByIdQuery;

public sealed record GetHealthPlanByIdQueryResponse(
    string Name,
    string AnsRegistrationCode,
    BeneficiaryMinimalDto[] Beneficiaries,
    DateTime CreationDate,
    DateTime? ModificationDate)
{
    public static GetHealthPlanByIdQueryResponse FromEntity(HealthPlan healthPlan)
        => new(
            healthPlan.Name,
            healthPlan.AnsRegistrationCode,
            [..healthPlan.Beneficiaries.Select(b => new BeneficiaryMinimalDto(b.Id, b.FullName))],
            healthPlan.CreatedAt,
            healthPlan.UpdatedAt
        );
}