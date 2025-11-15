using Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;

namespace Health.Api.Requests;

public sealed record UpdateHealthPlanRequest(
    string NewName,
    string NewAnsRegistrationCode)
{
    public UpdateHealthPlanCommand ToCommand(Guid id)
        => new(id, NewName, NewAnsRegistrationCode);
}