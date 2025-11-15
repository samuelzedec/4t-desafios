using System.Linq.Expressions;
using System.Net;
using Health.Application.Abstractions.Commands;
using Health.Application.Common;
using Health.Domain.Entities;
using Health.Domain.Repositories;

namespace Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;

public sealed class UpdateHealthPlanCommandHandler(
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateHealthPlanCommand, UpdateHealthPlanCommandResponse>
{
    public async Task<Result<UpdateHealthPlanCommandResponse>> Handle(
        UpdateHealthPlanCommand request,
        CancellationToken cancellationToken)
    {
        var healthPlan = await unitOfWork.HealthPlans
            .GetByIdAsync(request.HealthPlanId, cancellationToken);

        if (healthPlan is null)
            return Result.Failure<UpdateHealthPlanCommandResponse>("Plano de saúde não encontrado.", HttpStatusCode.NotFound);

        var validateConflicts = await ValidateHealthPlanConflicts(healthPlan, request, cancellationToken);
        if (!validateConflicts.IsSuccess) return validateConflicts;

        unitOfWork.HealthPlans.Update(healthPlan);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return UpdateHealthPlanCommandResponse.FromEntity(healthPlan);
    }

    private async Task<Result<UpdateHealthPlanCommandResponse>> ValidateHealthPlanConflicts(
        HealthPlan healthPlan,
        UpdateHealthPlanCommand request,
        CancellationToken cancellationToken)
    {
        var validationName = await ValidateNewValue(
            healthPlan.Name.Value,
            request.NewName,
            healthPlan.UpdateName,
            h => h.Name.Value == request.NewName,
            "O nome do plano de saúde já está em uso.",
            cancellationToken
        );

        if (!validationName.IsSuccess)
            return validationName;

        return await ValidateNewValue(
            healthPlan.AnsRegistrationCode.Value,
            request.NewAnsRegistrationCode,
            healthPlan.UpdateAnsCode,
            h => h.AnsRegistrationCode.Value == request.NewAnsRegistrationCode,
            "O código de registro ANS do plano de saúde já está em uso.",
            cancellationToken
        );
    }

    /// <summary>
    /// Valida um novo valor para uma propriedade de plano de saúde, verifica conflitos
    /// com registros existentes e atualiza a propriedade se a validação for bem-sucedida.
    /// </summary>
    /// <param name="currentValue">O valor atual da propriedade.</param>
    /// <param name="newValue">O novo valor a ser validado e potencialmente atualizado.</param>
    /// <param name="updateAction">A ação para aplicar o novo valor à propriedade correspondente.</param>
    /// <param name="errorMessage">A mensagem de erro a ser retornada em caso de conflito.</param>
    /// <param name="conflictExpression">
    /// Uma expressão usada para verificar conflitos com planos de saúde existentes.
    /// </param>
    /// <param name="cancellationToken">
    /// Um token para monitorar solicitações de cancelamento durante a operação assíncrona.
    /// </param>
    /// <returns>
    /// Um objeto de resultado indicando o sucesso ou falha da operação.
    /// Retorna um resultado de sucesso se não houver conflito e a propriedade for atualizada com sucesso.
    /// Retorna um resultado de falha se um conflito for detectado.
    /// </returns>
    private async Task<Result<UpdateHealthPlanCommandResponse>> ValidateNewValue(
        string currentValue,
        string newValue,
        Action<string> updateAction,
        Expression<Func<HealthPlan, bool>> conflictExpression,
        string errorMessage,
        CancellationToken cancellationToken)
    {
        if (ShouldSkipUpdate(currentValue, newValue))
            return Result.Success<UpdateHealthPlanCommandResponse>();

        if (await unitOfWork.HealthPlans.ExistsAsync(conflictExpression, cancellationToken))
            return Result.Failure<UpdateHealthPlanCommandResponse>(errorMessage, HttpStatusCode.Conflict);

        updateAction(newValue);
        return Result.Success<UpdateHealthPlanCommandResponse>();
    }

    /// <summary>
    /// Determina se uma operação de atualização deve ser ignorada
    /// com base na comparação do valor atual e do novo valor.
    /// </summary>
    /// <param name="currentValue">O valor existente da propriedade.</param>
    /// <param name="newValue">O novo valor a ser avaliado.</param>
    /// <returns>
    /// Um booleano indicando se a atualização deve ser ignorada.
    /// Retorna verdadeiro se o novo valor for nulo, vazio ou equivalente
    /// ao valor atual (sem distinção entre maiúsculas e minúsculas); caso contrário, falso.
    /// </returns>
    private static bool ShouldSkipUpdate(string currentValue, string? newValue) =>
        string.IsNullOrWhiteSpace(newValue) || currentValue.Equals(newValue, StringComparison.OrdinalIgnoreCase);
}