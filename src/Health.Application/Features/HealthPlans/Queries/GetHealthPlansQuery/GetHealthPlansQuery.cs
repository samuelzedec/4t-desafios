using Health.Application.Abstractions.Queries;
using Health.Application.Common;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlansQuery;

public sealed record GetHealthPlansQuery(
    GetHealthPlansQueryFilter Filter,
    int PageSize = 10,
    Guid? AfterKey = null
) : IQuery<KeysetPagedResult<GetHealthPlansQueryResponse>>;