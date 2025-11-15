using Health.Domain.Entities;

namespace Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;

public sealed record CreateHealthPlanCommandResponse(
    Guid Id,
    string Name,
    string AnsCode)
{
    public static CreateHealthPlanCommandResponse FromEntity(HealthPlan healthPlan)
        => new(healthPlan.Id, healthPlan.Name, healthPlan.AnsRegistrationCode);
}