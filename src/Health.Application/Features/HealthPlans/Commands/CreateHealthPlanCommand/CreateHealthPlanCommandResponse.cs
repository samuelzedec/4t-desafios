using Health.Application.Abstractions.Commands;
using Health.Domain.Entities;

namespace Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;

public sealed record CreateHealthPlanCommandResponse(
    Guid Id,
    string Name,
    string AnsCode
) : ICommandResponse
{
    public static CreateHealthPlanCommandResponse FromEntity(HealthPlan healthPlan)
        => new(healthPlan.Id, healthPlan.Name, healthPlan.AnsRegistrationCode);
}