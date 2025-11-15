using Health.Application.Abstractions.Commands;

namespace Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;

public sealed record UpdateHealthPlanCommandResponse(
    Guid Id,
    string Name,
    string AnsCode,
    DateTime CreationDate
) : ICommandResponse
{
    public static UpdateHealthPlanCommandResponse FromEntity(Health.Domain.Entities.HealthPlan healthPlan)
        => new(
            healthPlan.Id,
            healthPlan.Name,
            healthPlan.AnsRegistrationCode,
            healthPlan.CreatedAt
        );
}