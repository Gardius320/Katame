using FluentValidation.TestHelper;
using KatameApi.DTOs.Finance;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateCreditCardValidatorTests
{
    private readonly CreateCreditCardValidator _validator = new();

    private static CreateCreditCardDto Valid() => new()
    {
        Name = "Visa Gold",
        StatementDay = 5,
        PaymentDay = 20,
        CreditLimit = 1000,
        LogoDataUrl = null,
    };

    [Fact]
    public void Pasa_sin_logo_porque_es_opcional()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Pasa_con_un_logo_valido()
    {
        var dto = Valid();
        dto.LogoDataUrl = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAAB";
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.LogoDataUrl);
    }

    [Fact]
    public void Falla_si_el_logo_no_es_una_data_url_de_imagen()
    {
        var dto = Valid();
        dto.LogoDataUrl = "https://ejemplo.com/logo.png";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.LogoDataUrl);
    }

    [Fact]
    public void Falla_si_el_logo_es_demasiado_grande()
    {
        var dto = Valid();
        dto.LogoDataUrl = "data:image/png;base64," + new string('A', 700_000);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.LogoDataUrl);
    }

    [Fact]
    public void Pasa_sin_banco_porque_es_opcional()
    {
        var dto = Valid();
        dto.Bank = null;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Bank);
    }

    [Fact]
    public void Pasa_con_un_banco_valido()
    {
        var dto = Valid();
        dto.Bank = "Bancolombia";
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Bank);
    }

    [Fact]
    public void Falla_si_el_banco_supera_los_100_caracteres()
    {
        var dto = Valid();
        dto.Bank = new string('A', 101);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Bank);
    }

    [Fact]
    public void Pasa_con_datos_validos()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }
}
