using System.Net;
using Health.Application.Abstractions.Commands;
using Health.Application.Common;
using Health.Domain.Repositories;

namespace Health.Application.Features.HealthPlans.Commands.DeleteHealthPlanCommand;

internal sealed class DeleteHealthPlanCommandHandler(
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteHealthPlanCommand>
{
    public async Task<Result<EmptyResult>> Handle(
        DeleteHealthPlanCommand request,
        CancellationToken cancellationToken)
    {
        var healthPlan = await unitOfWork.HealthPlans
            .GetByIdAsync(request.HealthPlanId, cancellationToken);

        if (healthPlan is null)
            return Result.Failure("Plano de saúde não encontrado.", HttpStatusCode.NotFound);

        if (healthPlan.Beneficiaries.Count > 0)
            return Result.Failure("O plano de saúde não pode ser excluído pois possui beneficiários vinculados.");

        unitOfWork.HealthPlans.Delete(healthPlan);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}