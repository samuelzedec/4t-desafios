using System.Net;
using Health.Application.Abstractions.Commands;
using Health.Application.Common;
using Health.Domain.Entities;
using Health.Domain.Repositories;

namespace Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;

internal sealed class CreateBeneficiaryCommandHandler(
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateBeneficiaryCommand, CreateBeneficiaryCommandResponse>
{
    public async Task<Result<CreateBeneficiaryCommandResponse>> Handle(
        CreateBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateCpfBeneficiaryAsync(
            request.Cpf, cancellationToken);

        if (!validation.IsSuccess)
            return validation;

        var healthPlan = await unitOfWork.HealthPlans.GetByIdAsync(request.HealthPlanId, cancellationToken);
        if (healthPlan is null)
            return Result.Failure<CreateBeneficiaryCommandResponse>("Plano de saúde não existente.", HttpStatusCode.Conflict);

        var beneficiary = Beneficiary.Create(
            request.FullName,
            request.Cpf,
            request.BirthDate,
            request.HealthPlanId
        );

        await unitOfWork.Beneficiaries.CreateAsync(beneficiary, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(CreateBeneficiaryCommandResponse.Create(beneficiary, healthPlan), HttpStatusCode.Created);
    }

    private async Task<Result<CreateBeneficiaryCommandResponse>> ValidateCpfBeneficiaryAsync(
        string cpfRequest,
        CancellationToken cancellationToken)
    {
        var beneficiaryExists = await unitOfWork.Beneficiaries
            .ExistsAsync(b => b.Cpf.Value == cpfRequest, cancellationToken);

        return beneficiaryExists
            ? Result.Failure<CreateBeneficiaryCommandResponse>(
                "O CPF do beneficiário já está em uso.", HttpStatusCode.Conflict)
            : Result.Success<CreateBeneficiaryCommandResponse>();
    }
}