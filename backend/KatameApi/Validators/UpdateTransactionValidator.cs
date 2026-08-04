using FluentValidation;
using KatameApi.DTOs.Finance;
using KatameApi.Models;

namespace KatameApi.Validators;

public class UpdateTransactionValidator : AbstractValidator<UpdateTransactionDto>
{
    public UpdateTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("El tipo es obligatorio.")
            .Must(type => TransactionType.All.Contains(type))
            .WithMessage("Selecciona un tipo válido (ingreso o gasto).");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(50).WithMessage("La categoría no puede superar los 50 caracteres.");
    }
}
