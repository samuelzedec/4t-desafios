using System.Linq.Expressions;
using System.Net;
using Health.Application.Abstractions.Commands;
using Health.Application.Common;
using Health.Domain.Entities;
using Health.Domain.Repositories;

namespace Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;

internal sealed class CreateHealthPlanCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateHealthPlanCommand, CreateHealthPlanCommandResponse>
{
    public async Task<Result<CreateHealthPlanCommandResponse>> Handle(
        CreateHealthPlanCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await ValidateHealthPlanConflicts(request, cancellationToken);
        if (!validationResult.IsSuccess) return validationResult;

        var healthPlan = HealthPlan.Create(
            request.Name,
            request.AnsRegistrationCode
        );

        await unitOfWork.HealthPlans.CreateAsync(healthPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(CreateHealthPlanCommandResponse.FromEntity(healthPlan), HttpStatusCode.Created);
    }

    private async Task<Result<CreateHealthPlanCommandResponse>> ValidateHealthPlanConflicts(
        CreateHealthPlanCommand command,
        CancellationToken cancellationToken)
    {
        Dictionary<Expression<Func<HealthPlan, bool>>, string> validations = new()
        {
            { h => h.Name.Value == command.Name, "O nome do plano de saúde já está em uso." },
            { h => h.AnsRegistrationCode.Value == command.AnsRegistrationCode, "O código ANS já está em uso." }
        };

        foreach (var (predicate, errorMessage) in validations)
        {
            if (await unitOfWork.HealthPlans.ExistsAsync(predicate, cancellationToken))
                return Result.Failure<CreateHealthPlanCommandResponse>(errorMessage, HttpStatusCode.Conflict);
        }

        return Result.Success<CreateHealthPlanCommandResponse>();
    }
}