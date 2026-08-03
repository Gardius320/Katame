using FluentValidation;
using KatameApi.DTOs.Training;

namespace KatameApi.Validators;

public class CreateTrainingDayValidator : AbstractValidator<CreateTrainingDayDto>
{
    public CreateTrainingDayValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(100).WithMessage("El título no puede superar los 100 caracteres.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Selecciona un día de la semana válido.");
    }
}
