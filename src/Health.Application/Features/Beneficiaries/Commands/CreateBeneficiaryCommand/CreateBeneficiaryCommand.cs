using Health.Application.Abstractions.Commands;

namespace Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;

public sealed record CreateBeneficiaryCommand(
    string FullName,
    string Cpf,
    DateOnly BirthDate,
    Guid HealthPlanId
) : ICommand<CreateBeneficiaryCommandResponse>;