using FluentValidation;
using KatameApi.DTOs.Finance;

namespace KatameApi.Validators;

public class UpdateCreditCardValidator : AbstractValidator<UpdateCreditCardDto>
{
    public UpdateCreditCardValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.StatementDay)
            .InclusiveBetween(1, 31).WithMessage("El día de corte debe estar entre 1 y 31.");

        RuleFor(x => x.PaymentDay)
            .InclusiveBetween(1, 31).WithMessage("El día de pago debe estar entre 1 y 31.");

        RuleFor(x => x.CreditLimit)
            .GreaterThan(0).WithMessage("El límite de crédito debe ser mayor a cero.");

        RuleFor(x => x.LogoDataUrl)
            .Must(ImageDataUrlHelper.IsValidImageDataUrl).WithMessage("El logo debe ser una imagen válida (PNG o JPG).")
            .MaximumLength(700_000).WithMessage("La imagen del logo es demasiado grande.")
            .When(x => !string.IsNullOrEmpty(x.LogoDataUrl));

        RuleFor(x => x.Bank)
            .MaximumLength(100).WithMessage("El nombre del banco no puede superar los 100 caracteres.");
    }
}
