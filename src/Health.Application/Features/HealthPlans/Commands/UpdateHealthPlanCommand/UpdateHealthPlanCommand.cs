using Health.Application.Abstractions.Commands;

namespace Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;

public sealed record UpdateHealthPlanCommand(
    Guid HealthPlanId,
    string NewName,
    string NewAnsRegistrationCode
) : ICommand<UpdateHealthPlanCommandResponse>;