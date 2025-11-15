using FluentValidation;
using Health.Domain.ValueObjects.AnsRegistrationCode;
using Health.Domain.ValueObjects.Name;

namespace Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;

internal sealed class CreateHealthPlanCommandValidator
    : AbstractValidator<CreateHealthPlanCommand>
{
    public CreateHealthPlanCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .WithMessage("O nome do plano de saúde é obrigatório.")
            .NotEmpty()
            .WithMessage("O nome não pode estar em vazio.")
            .Length(Name.MinLength, Name.MaxLength)
            .WithMessage($"O nome deve conter entre {Name.MinLength} e {Name.MaxLength} caracteres.")
            .Matches(Name.RegexPatten)
            .WithMessage("O nome não pode conter números ou caracteres especiais.");

        RuleFor(c => c.AnsRegistrationCode)
            .NotNull()
            .WithMessage("O código de registro na ANS é obrigatório.")
            .NotEmpty()
            .WithMessage("O código de registro na ANS não pode estar em vazio.")
            .Length(AnsRegistrationCode.CodeLength)
            .WithMessage($"O código de registro na ANS deve conter exatamente {AnsRegistrationCode.CodeLength} caracteres.")
            .Matches(@"^[1-9]\d*$")
            .WithMessage("Código ANS deve conter apenas números e não pode começar com zero");
    }
}