using Health.Application.Abstractions.Commands;

namespace Health.Application.Features.HealthPlans.Commands.DeleteHealthPlanCommand;

public sealed record DeleteHealthPlanCommand(Guid HealthPlanId) : ICommand;