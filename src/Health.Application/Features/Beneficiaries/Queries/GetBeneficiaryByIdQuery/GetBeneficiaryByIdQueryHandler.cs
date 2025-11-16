using System.Net;
using Health.Application.Abstractions.Queries;
using Health.Application.Common;
using Health.Domain.Repositories;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaryByIdQuery;

internal sealed class GetBeneficiaryByIdQueryHandler(
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetBeneficiaryByIdQuery, GetBeneficiaryByIdQueryResponse>
{
    public async Task<Result<GetBeneficiaryByIdQueryResponse>> Handle(
        GetBeneficiaryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var beneficiary = await unitOfWork.Beneficiaries
            .GetByIdWithHealthPlanAsync(request.Id, cancellationToken);

        return beneficiary is not null
            ? GetBeneficiaryByIdQueryResponse.FromEntity(beneficiary)
            : Result.Failure<GetBeneficiaryByIdQueryResponse>("Beneficiário não encontrado.", HttpStatusCode.NotFound);
    }
}