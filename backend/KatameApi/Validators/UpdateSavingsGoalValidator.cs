using FluentValidation;
using KatameApi.DTOs.Finance;

namespace KatameApi.Validators;

public class UpdateSavingsGoalValidator : AbstractValidator<UpdateSavingsGoalDto>
{
    public UpdateSavingsGoalValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.TargetAmount)
            .GreaterThan(0).WithMessage("La meta debe ser mayor a cero.");

        RuleFor(x => x.CurrentAmount)
            .GreaterThanOrEqualTo(0).WithMessage("El monto actual no puede ser negativo.");
    }
}
