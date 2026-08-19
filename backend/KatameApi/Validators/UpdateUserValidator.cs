using FluentValidation;
using KatameApi.DTOs.Users;

namespace KatameApi.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");

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

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(@"^3\d{9}$").WithMessage("Ingresa un celular colombiano válido (10 dígitos, empieza en 3).");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("Ingresa un email válido.")
            .MaximumLength(150).WithMessage("El email no puede superar los 150 caracteres.");

        RuleFor(x => x.Password)
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}
