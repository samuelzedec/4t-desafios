using System.Net;
using Health.Application.Abstractions.Queries;
using Health.Application.Common;
using Health.Domain.Repositories;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlanByIdQuery;

public sealed class GetHealthPlanByIdQueryHandler(
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetHealthPlanByIdQuery, GetHealthPlanByIdQueryResponse>
{
    public async Task<Result<GetHealthPlanByIdQueryResponse>> Handle(
        GetHealthPlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        var healthPlan = await unitOfWork.HealthPlans
            .GetByIdWithBeneficiariesAsync(request.Id, cancellationToken);

        return healthPlan is not null
            ? GetHealthPlanByIdQueryResponse.FromEntity(healthPlan)
            : Result.Failure<GetHealthPlanByIdQueryResponse>("Plano de saúde não encontrado.", HttpStatusCode.NotFound);
    }
}