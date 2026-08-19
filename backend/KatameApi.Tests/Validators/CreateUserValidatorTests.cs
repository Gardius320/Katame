using FluentValidation.TestHelper;
using KatameApi.DTOs.Users;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    private static CreateUserDto Valid() => new()
    {
        Username = "ana",
        FirstName = "Ana",
        LastName = "Pérez",
        DocumentId = "1020304050",
        PhoneNumber = "3001234567",
        Email = "ana@katame.local",
        Password = "Password123!",
    };

    [Fact]
    public void Falla_cuando_el_username_esta_vacio()
    {
        var dto = Valid();
        dto.Username = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Falla_cuando_el_email_no_es_valido()
    {
        var dto = Valid();
        dto.Email = "no-es-un-email";
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

    [Fact]
    public void Falla_cuando_la_cedula_esta_vacia()
    {
        var dto = Valid();
        dto.DocumentId = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }

    [Theory]
    [InlineData("12345")] // longitud incorrecta (menos de 6 dígitos)
    [InlineData("12345678901")] // longitud incorrecta (más de 10 dígitos)
    [InlineData("0123456789")] // no puede empezar en 0
    [InlineData("10ABCDEFGH")] // no numérica
    public void Falla_cuando_la_cedula_no_es_valida(string documentId)
    {
        var dto = Valid();
        dto.DocumentId = documentId;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789")] // muy corto (9 dígitos)
    [InlineData("30012345678")] // muy largo (11 dígitos)
    [InlineData("2001234567")] // no empieza en 3
    public void Falla_cuando_el_telefono_no_es_valido(string phoneNumber)
    {
        var dto = Valid();
        dto.PhoneNumber = phoneNumber;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("3001234567")]
    [InlineData("3159876543")]
    public void Pasa_con_telefonos_validos(string phoneNumber)
    {
        var dto = Valid();
        dto.PhoneNumber = phoneNumber;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Pasa_con_datos_validos()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }
}
