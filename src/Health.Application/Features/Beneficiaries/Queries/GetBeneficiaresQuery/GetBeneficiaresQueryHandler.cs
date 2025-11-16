using Health.Application.Abstractions.Queries;
using Health.Application.Common;
using Health.Domain.Repositories;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaresQuery;

internal sealed class GetBeneficiaresQueryHandler(
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetBeneficiaresQuery, KeysetPagedResult<GetBeneficiaresQueryResponse>>
{
    public async Task<Result<KeysetPagedResult<GetBeneficiaresQueryResponse>>> Handle(
        GetBeneficiaresQuery request,
        CancellationToken cancellationToken)
    {
        var pagedBeneficiaries = await unitOfWork.Beneficiaries.GetPagedAsync(
            request.Filter,
            request.PageSize,
            request.AfterKey,
            cancellationToken
        );

        var response = GetBeneficiaresQueryResponse
            .FromEntity(pagedBeneficiaries);

        return KeysetPagedResult<GetBeneficiaresQueryResponse>.Create(
            items: response,
            pageSize: request.PageSize,
            keySelector: h => h.Id,
            hasPreviousPage: request.AfterKey.HasValue
        );
    }
}