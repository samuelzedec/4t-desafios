using System.Net;
using Health.Application.Abstractions.Commands;
using Health.Application.Common;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Shared;

namespace Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;

internal sealed class UpdateBeneficiaryCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateBeneficiaryCommand, UpdateBeneficiaryCommandResponse>
{
    public async Task<Result<UpdateBeneficiaryCommandResponse>> Handle(
        UpdateBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiary = await unitOfWork.Beneficiaries.GetByIdAsync(request.Id, cancellationToken);
        if (beneficiary is null)
            return Result.Failure<UpdateBeneficiaryCommandResponse>("Beneficiário não encontrado.", HttpStatusCode.NotFound);

        var cpfResult = await UpdateCpfIfValidAsync(beneficiary, request.Cpf, cancellationToken);
        if (!cpfResult.IsSuccess)
            return cpfResult;

        var healthPlanResult = await UpdateHealthPlanIfExistsAsync(beneficiary, request.HealthPlanId, cancellationToken);
        if (!healthPlanResult.IsSuccess)
            return healthPlanResult;

        UpdateBasicProperties(beneficiary, request);
        unitOfWork.Beneficiaries.Update(beneficiary);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(UpdateBeneficiaryCommandResponse.FromEntity(beneficiary));
    }

    private async Task<Result<UpdateBeneficiaryCommandResponse>> UpdateCpfIfValidAsync(
        Beneficiary beneficiary,
        string cpf,
        CancellationToken cancellationToken)
    {
        if (UpdateHelper.ShouldSkipUpdate(beneficiary.Cpf.Value, cpf))
            return Result.Success<UpdateBeneficiaryCommandResponse>();

        if (await unitOfWork.Beneficiaries.ExistsAsync(b => b.Cpf.Value == cpf, cancellationToken))
            return Result.Failure<UpdateBeneficiaryCommandResponse>("O CPF do beneficiário já está em uso.", HttpStatusCode.Conflict);

        beneficiary.UpdateCpf(cpf);
        return Result.Success<UpdateBeneficiaryCommandResponse>();
    }

    private async Task<Result<UpdateBeneficiaryCommandResponse>> UpdateHealthPlanIfExistsAsync(
        Beneficiary beneficiary,
        Guid? healthPlanId,
        CancellationToken cancellationToken)
    {
        if (UpdateHelper.ShouldSkipUpdate(beneficiary.HealthPlanId, healthPlanId))
            return Result.Success<UpdateBeneficiaryCommandResponse>();

        if (!await unitOfWork.HealthPlans.ExistsAsync(h => h.Id == healthPlanId, cancellationToken))
            return Result.Failure<UpdateBeneficiaryCommandResponse>("O plano de saúde informado não existe.", HttpStatusCode.NotFound);

        beneficiary.UpdateHealthPlan(healthPlanId!.Value);
        return Result.Success<UpdateBeneficiaryCommandResponse>();
    }

    private static void UpdateBasicProperties(
        Beneficiary beneficiary,
        UpdateBeneficiaryCommand command)
    {
        if (!UpdateHelper.ShouldSkipUpdate(beneficiary.BirthDate, command.BirthDate))
            beneficiary.UpdateBirthDate(command.BirthDate!.Value);

        if (!UpdateHelper.ShouldSkipUpdate(beneficiary.Status, command.Status))
            beneficiary.UpdateStatus(command.Status!.Value);

        if (!UpdateHelper.ShouldSkipUpdate(beneficiary.FullName, command.FullName))
            beneficiary.UpdateFullName(command.FullName);
    }
}