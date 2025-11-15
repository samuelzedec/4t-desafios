using FluentValidation;
using Health.Domain.ValueObjects.AnsRegistrationCode;
using Health.Domain.ValueObjects.Name;

namespace Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;

internal class UpdateHealthPlanCommandValidator
    : AbstractValidator<UpdateHealthPlanCommand>
{
    public UpdateHealthPlanCommandValidator()
    {
        RuleFor(c => c.HealthPlanId)
            .NotNull()
            .WithMessage("O Id do plano de saúde é obrigatório.")
            .NotEmpty()
            .WithMessage("O Id do plano de saúde não pode estar vazio.");

        When(h => !string.IsNullOrWhiteSpace(h.NewName), () =>
            RuleFor(c => c.NewName)
                .Length(Name.MinLength, Name.MaxLength)
                .WithMessage($"O nome deve conter entre {Name.MinLength} e {Name.MaxLength} caracteres.")
                .Matches(Name.RegexPatten)
                .WithMessage("O nome não pode conter números ou caracteres especiais.")
        );

        When(h => !string.IsNullOrWhiteSpace(h.NewAnsRegistrationCode), () =>
            RuleFor(c => c.NewAnsRegistrationCode)
                .Length(AnsRegistrationCode.CodeLength)
                .WithMessage($"O código de registro na ANS deve conter exatamente {AnsRegistrationCode.CodeLength} caracteres.")
                .Matches(@"^[1-9]\d*$")
                .WithMessage("Código ANS deve conter apenas números e não pode começar com zero")
        );
    }
}