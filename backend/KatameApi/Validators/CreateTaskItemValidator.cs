using FluentValidation;
using KatameApi.DTOs.Tasks;
using KatameApi.Models;

namespace KatameApi.Validators;

public class CreateTaskItemValidator : AbstractValidator<CreateTaskItemDto>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(150).WithMessage("El título no puede superar los 150 caracteres.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(status => TaskItemStatus.All.Contains(status))
            .WithMessage("Selecciona un estado válido.");
    }
}
