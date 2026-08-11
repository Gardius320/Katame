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
            .Must(EcuadorianDocumentId.IsValidCedula).WithMessage("Ingresa una cédula ecuatoriana válida.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("Ingresa un correo válido.")
            .MaximumLength(150).WithMessage("El correo no puede superar los 150 caracteres.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(@"^(09\d{8}|0[2-7]\d{7})$").WithMessage("Ingresa un teléfono ecuatoriano válido (celular: 09XXXXXXXX, fijo: 0X XXXXXXX).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}
