using FluentValidation;

namespace Health.Application.Features.HealthPlans.Commands.DeleteHealthPlanCommand;

internal sealed class DeleteHealthPlanCommandValidator
    : AbstractValidator<DeleteHealthPlanCommand>
{
    public DeleteHealthPlanCommandValidator()
    {
        RuleFor(c => c.HealthPlanId)
            .NotNull()
            .WithMessage("O Id do plano de saúde é obrigatório.")
            .NotEmpty()
            .WithMessage("O Id do plano de saúde não pode estar vazio.");
    }
}