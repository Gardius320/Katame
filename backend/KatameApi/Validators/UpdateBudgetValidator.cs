using FluentValidation;
using KatameApi.DTOs.Finance;
using KatameApi.Models;

namespace KatameApi.Validators;

public class UpdateBudgetValidator : AbstractValidator<UpdateBudgetDto>
{
    public UpdateBudgetValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(50).WithMessage("La categoría no puede superar los 50 caracteres.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto del presupuesto debe ser mayor a cero.");

        RuleFor(x => x.Period)
            .Must(period => BudgetPeriod.All.Contains(period))
            .WithMessage("La periodicidad debe ser semanal, quincenal o mensual.");

        RuleFor(x => x.AnchorDate)
            .NotEqual(default(DateTime)).WithMessage("Debes indicar cuándo empieza el ciclo.");
    }
}
