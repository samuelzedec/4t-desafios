using FluentValidation;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlansQuery;

public sealed class GetHealthPlansQueryValidator
    : AbstractValidator<GetHealthPlansQuery>
{
    public GetHealthPlansQueryValidator()
    {
        RuleFor(q => q.PageSize)
            .GreaterThan(0)
            .WithMessage("A quantidade requisitada de dados deve ser maior que zero.");
    }
}