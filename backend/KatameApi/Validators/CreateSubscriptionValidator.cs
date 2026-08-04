using FluentValidation;
using KatameApi.DTOs.Subscriptions;

namespace KatameApi.Validators;

public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
    }
}
