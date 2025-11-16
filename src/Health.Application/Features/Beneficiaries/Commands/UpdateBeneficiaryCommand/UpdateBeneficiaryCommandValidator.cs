using FluentValidation;
using Health.Domain.ValueObjects.BirthDate;
using Health.Domain.ValueObjects.Cpf;
using Health.Domain.ValueObjects.Name;

namespace Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;

internal sealed class UpdateBeneficiaryCommandValidator
    : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .WithMessage("O Id do beneficiário é obrigatório.")
            .NotEmpty()
            .WithMessage("O Id do beneficiário não pode estar vazio.");
        
        When(c => !string.IsNullOrWhiteSpace(c.FullName), () =>
            RuleFor(c => c.FullName)
                .Length(Name.MinLength, Name.MaxLength)
                .WithMessage($"O nome completo deve conter entre {Name.MinLength} e {Name.MaxLength} caracteres.")
                .Matches(Name.RegexPatten)
                .WithMessage("O nome completo não pode conter números ou caracteres especiais.")
        );

        When(c => !string.IsNullOrWhiteSpace(c.Cpf), () =>
            RuleFor(c => c.Cpf)
                .Matches(Cpf.RegexPattern)
                .WithMessage("CPF deve conter apenas números")
                .Length(Cpf.Length)
                .WithMessage($"O CPF deve conter exatamente {Cpf.Length} caracteres.")
        );

        When(c => c.BirthDate.HasValue, () =>
            RuleFor(c => c.BirthDate!.Value)
                .Must(date =>
                {
                    var calculateAge = BirthDate.CalculateAge(date);
                    return calculateAge <= BirthDate.MaximumAge;
                })
                .WithMessage($"A idade não pode ser maior que {BirthDate.MaximumAge} anos.")
        );

        When(c => c.Status.HasValue, () =>
            RuleFor(x => x.Status!.Value)
                .IsInEnum()
                .WithMessage("O status informado é inválido.")
        );
    }
}