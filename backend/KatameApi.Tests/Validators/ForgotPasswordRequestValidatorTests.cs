using FluentValidation.TestHelper;
using KatameApi.DTOs.Auth;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordRequestValidator _validator = new();

    [Fact]
    public void Falla_cuando_el_correo_esta_vacio()
    {
        var result = _validator.TestValidate(new ForgotPasswordRequestDto { Email = "" });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Falla_cuando_el_correo_no_es_valido()
    {
        var result = _validator.TestValidate(new ForgotPasswordRequestDto { Email = "no-es-un-correo" });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Pasa_con_un_correo_valido()
    {
        var result = _validator.TestValidate(new ForgotPasswordRequestDto { Email = "ana@correo.com" });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
