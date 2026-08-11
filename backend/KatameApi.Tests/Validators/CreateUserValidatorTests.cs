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
        DocumentId = "1701234567",
        PhoneNumber = "0999999999",
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
    [InlineData("1234567890")] // dígito verificador incorrecto
    [InlineData("9901234567")] // código de provincia inválido (99)
    [InlineData("1791234567")] // tercer dígito de persona natural inválido (9)
    [InlineData("12345")] // longitud incorrecta
    [InlineData("17ABCDEFGH")] // no numérica
    public void Falla_cuando_la_cedula_no_es_valida(string documentId)
    {
        var dto = Valid();
        dto.DocumentId = documentId;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789")] // muy corto
    [InlineData("0512345678")] // prefijo de celular inválido
    [InlineData("08123456789")] // prefijo de fijo inválido
    public void Falla_cuando_el_telefono_no_es_valido(string phoneNumber)
    {
        var dto = Valid();
        dto.PhoneNumber = phoneNumber;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("0991234567")] // celular
    [InlineData("022345678")] // fijo
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
