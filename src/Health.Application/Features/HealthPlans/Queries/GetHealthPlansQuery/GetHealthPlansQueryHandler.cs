using Health.Application.Abstractions.Queries;
using Health.Application.Common;
using Health.Domain.Repositories;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlansQuery;

public sealed class GetHealthPlansQueryHandler(
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetHealthPlansQuery, KeysetPagedResult<GetHealthPlansQueryResponse>>
{
    public async Task<Result<KeysetPagedResult<GetHealthPlansQueryResponse>>> Handle(
        GetHealthPlansQuery request,
        CancellationToken cancellationToken)
    {
        var pagedHealthPlans = await unitOfWork.HealthPlans.GetPagedAsync(
            request.Filter,
            request.PageSize,
            request.AfterKey,
            cancellationToken
        );

        var response = GetHealthPlansQueryResponse
            .FromEntity(pagedHealthPlans);

        return KeysetPagedResult<GetHealthPlansQueryResponse>.Create(
            items: response,
            pageSize: request.PageSize,
            keySelector: h => h.Id,
            hasPreviousPage: request.AfterKey.HasValue
        );
    }
}