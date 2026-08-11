using FluentValidation.TestHelper;
using KatameApi.DTOs.Auth;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    private static RegisterRequestDto Valid() => new()
    {
        FirstName = "Ana",
        LastName = "Pérez",
        DocumentId = "1701234567",
        Email = "ana@correo.com",
        PhoneNumber = "0991234567",
        Password = "Password123!",
    };

    [Fact]
    public void Falla_cuando_el_nombre_esta_vacio()
    {
        var dto = Valid();
        dto.FirstName = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Falla_cuando_el_apellido_esta_vacio()
    {
        var dto = Valid();
        dto.LastName = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Falla_cuando_el_correo_no_es_valido()
    {
        var dto = Valid();
        dto.Email = "no-es-un-correo";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Falla_cuando_la_password_es_muy_corta()
    {
        var dto = Valid();
        dto.Password = "123";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("1234567890")] // dígito verificador incorrecto
    [InlineData("9901234567")] // provincia inválida
    [InlineData("")]
    public void Falla_cuando_la_cedula_no_es_valida(string documentId)
    {
        var dto = Valid();
        dto.DocumentId = documentId;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("0512345678")]
    public void Falla_cuando_el_telefono_no_es_valido(string phoneNumber)
    {
        var dto = Valid();
        dto.PhoneNumber = phoneNumber;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Pasa_con_datos_validos()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }
}
