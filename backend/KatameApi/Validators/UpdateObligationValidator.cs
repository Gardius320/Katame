using FluentValidation;
using KatameApi.DTOs.Finance;

namespace KatameApi.Validators;

public class UpdateObligationValidator : AbstractValidator<UpdateObligationDto>
{
    public UpdateObligationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.RecurrenceFrequency)
            .NotNull().WithMessage("Selecciona si la obligación es quincenal o mensual.")
            .When(x => x.IsRecurring);
    }
}
