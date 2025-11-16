using FluentValidation;
using Health.Domain.ValueObjects.BirthDate;
using Health.Domain.ValueObjects.Cpf;
using Health.Domain.ValueObjects.Name;

namespace Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;

internal sealed class CreateBeneficiaryCommandValidator
    : AbstractValidator<CreateBeneficiaryCommand>
{
    public CreateBeneficiaryCommandValidator()
    {
        RuleFor(c => c.FullName)
            .NotNull()
            .WithMessage("O nome completo do beneficiário é obrigatório.")
            .NotEmpty()
            .WithMessage("O nome completo não pode estar em vazio.")
            .Length(Name.MinLength, Name.MaxLength)
            .WithMessage($"O nome completo deve conter entre {Name.MinLength} e {Name.MaxLength} caracteres.")
            .Matches(Name.RegexPatten)
            .WithMessage("O nome completo não pode conter números ou caracteres especiais.");

        RuleFor(c => c.Cpf)
            .NotNull()
            .WithMessage("O CPF do beneficiário é obrigatório.")
            .NotEmpty()
            .WithMessage("O CPF do beneficiário não pode estar em vazio.")
            .Matches(Cpf.RegexPattern)
            .WithMessage($"O CPF deve conter exatamente {Cpf.Length} números.");

        RuleFor(c => c.BirthDate)
            .NotNull()
            .WithMessage("A data de nascimento é obrigatória.")
            .NotEmpty()
            .WithMessage("A data de nascimento não pode estar em vazia.")
            .Must(date =>
            {
                var calculateAge = BirthDate.CalculateAge(date);
                return calculateAge <= BirthDate.MaximumAge;
            })
            .WithMessage($"A idade não pode ser maior que {BirthDate.MaximumAge} anos.");

        RuleFor(c => c.HealthPlanId)
            .NotNull()
            .WithMessage("É obrigatório beneficiário estar vinculado a um plano de saúde.")
            .NotEmpty()
            .WithMessage("O plano de saúde do beneficiário não pode estar em vazio.");
    }
}