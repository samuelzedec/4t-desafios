using Health.Domain.Abstractions;
using Health.Domain.Entities;
using Health.Domain.Enums;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaresQuery;

public sealed class GetBeneficiaresQueryFilter : IFilter<Beneficiary>
{
    public string? FullName { get; set; }
    public string? Cpf { get; set; }
    public Status? Status { get; set; }
    public Guid? HealthPlanId { get; set; }
    public DateOnly? BirthDate { get; set; }

    public IQueryable<Beneficiary> Apply(IQueryable<Beneficiary> query)
    {
        if (!string.IsNullOrWhiteSpace(FullName))
            query = query.Where(b => b.FullName.Value.Contains(FullName));

        if (!string.IsNullOrWhiteSpace(Cpf))
            query = query.Where(b => b.Cpf.Value.Contains(Cpf));

        if (Status.HasValue)
            query = query.Where(b => b.Status == Status.Value);

        if (HealthPlanId.HasValue)
            query = query.Where(b => b.HealthPlanId == HealthPlanId.Value);

        if (BirthDate.HasValue)
            query = query.Where(b => b.BirthDate.Value == BirthDate.Value);

        return query;
    }
}