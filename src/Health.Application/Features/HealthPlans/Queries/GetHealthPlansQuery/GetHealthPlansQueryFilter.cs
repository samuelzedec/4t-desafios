using Health.Domain.Abstractions;
using Health.Domain.Entities;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlansQuery;

public sealed class GetHealthPlansQueryFilter : IFilter<HealthPlan>
{
    public string? Name { get; set; }
    public string? AnsCode { get; set; }

    public IQueryable<HealthPlan> Apply(IQueryable<HealthPlan> query)
    {
        if (!string.IsNullOrWhiteSpace(Name))
            query = query.Where(h => h.Name.Value.Contains(Name));

        if (!string.IsNullOrWhiteSpace(AnsCode))
            query = query.Where(h => h.AnsRegistrationCode.Value.Contains(AnsCode));

        return query;
    }
}