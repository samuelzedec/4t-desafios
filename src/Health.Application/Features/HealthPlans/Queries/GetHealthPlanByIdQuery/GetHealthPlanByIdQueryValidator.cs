using FluentValidation;

namespace Health.Application.Features.HealthPlans.Queries.GetHealthPlanByIdQuery;

public sealed class GetHealthPlanByIdQueryValidator 
    : AbstractValidator<GetHealthPlanByIdQuery>
{
    public GetHealthPlanByIdQueryValidator()
    {
        RuleFor(h => h.Id)
            .NotNull()
            .WithMessage("O Id do plano de saúde é obrigatório.")
            .NotEmpty()
            .WithMessage("O Id do plano de saúde não pode estar vazio.");
    }
}