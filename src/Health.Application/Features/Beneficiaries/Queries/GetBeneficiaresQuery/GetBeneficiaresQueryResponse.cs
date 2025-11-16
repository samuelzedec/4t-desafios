using Health.Application.Extensions;
using Health.Domain.Entities;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaresQuery;

public sealed record GetBeneficiaresQueryResponse(
    Guid Id,
    string FullName,
    string Cpf,
    string BirthDate,
    string Status)
{
    public static List<GetBeneficiaresQueryResponse> FromEntity(List<Beneficiary> beneficiaries)
        =>
        [
            .. beneficiaries.Select(b => new GetBeneficiaresQueryResponse(
                b.Id,
                b.FullName,
                b.Cpf,
                b.BirthDate,
                b.Status.GetDescription()
            ))
        ];
}