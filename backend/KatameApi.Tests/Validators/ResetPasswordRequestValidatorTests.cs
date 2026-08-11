using FluentValidation.TestHelper;
using KatameApi.DTOs.Auth;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void Falla_cuando_el_token_esta_vacio()
    {
        var result = _validator.TestValidate(new ResetPasswordRequestDto { Token = "", NewPassword = "Password123!" });
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void Falla_cuando_la_password_es_muy_corta()
    {
        var result = _validator.TestValidate(new ResetPasswordRequestDto { Token = "abc", NewPassword = "123" });
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Pasa_con_datos_validos()
    {
        var result = _validator.TestValidate(new ResetPasswordRequestDto { Token = "abc", NewPassword = "Password123!" });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
