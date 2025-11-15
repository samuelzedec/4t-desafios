using Health.Application.Abstractions.Queries;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlanByIdQuery;

public sealed record GetHealthPlanByIdQuery(Guid Id)
    : IQuery<GetHealthPlanByIdQueryResponse>;