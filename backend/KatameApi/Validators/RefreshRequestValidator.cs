using FluentValidation;
using KatameApi.DTOs.Auth;

namespace KatameApi.Validators;

public class RefreshRequestValidator : AbstractValidator<RefreshRequestDto>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("El token de renovación es obligatorio.");
    }
}
