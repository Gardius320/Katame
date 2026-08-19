using FluentValidation.TestHelper;
using KatameApi.DTOs.Users;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _validator = new();

    private static UpdateUserDto Valid() => new()
    {
        Username = "ana",
        FirstName = "Ana",
        LastName = "Pérez",
        DocumentId = "1020304050",
        PhoneNumber = "3001234567",
        Email = "ana@katame.local",
    };

    [Fact]
    public void Falla_cuando_el_email_no_es_valido()
    {
        var dto = Valid();
        dto.Email = "no-es-un-email";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Pasa_sin_password_porque_es_opcional()
    {
        var dto = Valid();
        dto.Password = null;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Falla_si_la_password_viene_pero_es_muy_corta()
    {
        var dto = Valid();
        dto.Password = "123";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Falla_cuando_la_cedula_no_tiene_un_formato_valido()
    {
        var dto = Valid();
        dto.DocumentId = "0123456789"; // no puede empezar en 0
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }

    [Fact]
    public void Falla_cuando_el_telefono_no_tiene_un_formato_valido()
    {
        var dto = Valid();
        dto.PhoneNumber = "12345";
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
