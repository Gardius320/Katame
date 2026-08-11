using FluentValidation;
using KatameApi.DTOs.Auth;

namespace KatameApi.Validators;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("Ingresa un correo válido.");
    }
}
