using Health.Domain.Entities;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlansQuery;

public sealed record GetHealthPlansQueryResponse(
    Guid Id,
    string Name,
    string AnsRegistrationCode)
{
    public static List<GetHealthPlansQueryResponse> FromEntity(List<HealthPlan> healthPlan)
        => [.. healthPlan.Select(h => new GetHealthPlansQueryResponse(h.Id, h.Name, h.AnsRegistrationCode))];
}