using Health.Application.Abstractions.Commands;

namespace Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;

public sealed record CreateHealthPlanCommand(
    string Name,
    string AnsRegistrationCode
) : ICommand<CreateHealthPlanCommandResponse>;