using Health.Application.Abstractions.Queries;
using Health.Application.Common;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaresQuery;

public sealed record GetBeneficiaresQuery(
    GetBeneficiaresQueryFilter Filter,
    int PageSize,
    Guid? AfterKey = null
) : IQuery<KeysetPagedResult<GetBeneficiaresQueryResponse>>;