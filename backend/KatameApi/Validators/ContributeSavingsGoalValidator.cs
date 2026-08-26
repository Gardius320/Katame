using FluentValidation;
using KatameApi.DTOs.Finance;

namespace KatameApi.Validators;

public class ContributeSavingsGoalValidator : AbstractValidator<ContributeSavingsGoalDto>
{
    public ContributeSavingsGoalValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto a agregar debe ser mayor a cero.");
    }
}
