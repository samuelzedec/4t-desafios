using Health.Application.Abstractions.Commands;
using Health.Domain.Enums;

namespace Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;

public sealed record UpdateBeneficiaryCommand(
    Guid Id,
    string FullName,
    string Cpf,
    DateOnly? BirthDate,
    Guid? HealthPlanId,
    Status? Status
) : ICommand<UpdateBeneficiaryCommandResponse>;