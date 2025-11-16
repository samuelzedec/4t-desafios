using FluentValidation;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaryByIdQuery;

internal sealed class GetBeneficiaryByIdQueryValidator 
    : AbstractValidator<GetBeneficiaryByIdQuery>
{
    public GetBeneficiaryByIdQueryValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .WithMessage("O Id do beneficiário é obrigatório.")
            .NotEmpty()
            .WithMessage("O Id do beneficiário não pode estar vazio.");
    }
}