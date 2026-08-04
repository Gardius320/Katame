using FluentValidation;
using KatameApi.DTOs.Goals;

namespace KatameApi.Validators;

public class UpdateGoalValidator : AbstractValidator<UpdateGoalDto>
{
    public UpdateGoalValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(150).WithMessage("El título no puede superar los 150 caracteres.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(50).WithMessage("La categoría no puede superar los 50 caracteres.");

        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100).WithMessage("El progreso debe estar entre 0 y 100.");
    }
}
