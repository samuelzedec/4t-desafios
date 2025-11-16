using FluentValidation;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaresQuery;

public sealed class GetBeneficiaresQueryValidator 
    : AbstractValidator<GetBeneficiaresQuery>
{
    public GetBeneficiaresQueryValidator()
    {
        RuleFor(q => q.PageSize)
            .GreaterThan(0)
            .WithMessage("A quantidade requisitada de dados deve ser maior que zero.");
    }
}