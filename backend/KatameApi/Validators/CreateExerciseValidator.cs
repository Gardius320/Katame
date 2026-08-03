using FluentValidation;
using KatameApi.DTOs.Training;

namespace KatameApi.Validators;

public class CreateExerciseValidator : AbstractValidator<CreateExerciseDto>
{
    public CreateExerciseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del ejercicio es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.SetsReps)
            .NotEmpty().WithMessage("Las series y repeticiones son obligatorias.")
            .MaximumLength(50).WithMessage("Las series y repeticiones no pueden superar los 50 caracteres.");
    }
}
