using Health.Application.Abstractions.Commands;

namespace Health.Application.Features.Beneficiaries.Commands.DeleteBeneficiaryCommand;

public sealed record DeleteBeneficiaryCommand(Guid Id) : ICommand;