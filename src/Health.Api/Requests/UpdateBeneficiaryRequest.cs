using Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;
using Health.Domain.Enums;

namespace Health.Api.Requests;

public sealed record UpdateBeneficiaryRequest(
    string FullName,
    string Cpf,
    DateOnly? BirthDate,
    Guid? HealthPlanId,
    Status? Status)
{
    public UpdateBeneficiaryCommand ToCommand(Guid id)
        => new(id, FullName, Cpf, BirthDate, HealthPlanId, Status);
}