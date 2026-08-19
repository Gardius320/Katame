using FluentValidation;
using KatameApi.DTOs.Auth;

namespace KatameApi.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");

        RuleFor(x => x.DocumentId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La cédula es obligatoria.")
            .Must(ColombianDocumentId.IsValidCedula).WithMessage("Ingresa una cédula colombiana válida.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("Ingresa un correo válido.")
            .MaximumLength(150).WithMessage("El correo no puede superar los 150 caracteres.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(@"^3\d{9}$").WithMessage("Ingresa un celular colombiano válido (10 dígitos, empieza en 3).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}
