using Health.Application.Extensions;
using Health.Domain.Entities;
using Health.Domain.Enums;

namespace Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;

public sealed record UpdateBeneficiaryCommandResponse(
    Guid Id,
    string FullName,
    string Cpf,
    string BirthDate,
    string Status,
    DateTime CreationDate
)
{
    public static UpdateBeneficiaryCommandResponse FromEntity(Beneficiary beneficiary)
        => new(
            beneficiary.Id,
            beneficiary.FullName.Value,
            beneficiary.Cpf.Value,
            beneficiary.BirthDate,
            beneficiary.Status.GetDescription(),
            beneficiary.CreatedAt
        );
};