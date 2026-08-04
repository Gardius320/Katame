using FluentValidation;
using KatameApi.DTOs.Projects;
using KatameApi.Models;

namespace KatameApi.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(status => ProjectStatus.All.Contains(status))
            .WithMessage("Selecciona un estado válido.");
    }
}
