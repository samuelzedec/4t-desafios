using FluentValidation;

namespace Health.Application.Features.Beneficiaries.Commands.DeleteBeneficiaryCommand;

internal sealed class DeleteBeneficiaryCommandValidator
    : AbstractValidator<DeleteBeneficiaryCommand>
{
    public DeleteBeneficiaryCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .WithMessage("O Id do beneficiário é obrigatório.")
            .NotEmpty()
            .WithMessage("O Id do beneficiário não pode estar vazio.");
    }
}