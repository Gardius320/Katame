using FluentValidation.TestHelper;
using KatameApi.DTOs.Finance;
using KatameApi.Models;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class UpdateBudgetValidatorTests
{
    private readonly UpdateBudgetValidator _validator = new();

    private static UpdateBudgetDto Valid() => new()
    {
        Category = "Comida",
        Amount = 500_000,
        Period = BudgetPeriod.Monthly,
        AnchorDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
    };

    [Fact]
    public void Pasa_con_datos_validos()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Falla_si_la_categoria_esta_vacia()
    {
        var dto = Valid();
        dto.Category = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Falla_si_el_monto_no_es_mayor_a_cero()
    {
        var dto = Valid();
        dto.Amount = -1;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Falla_si_la_periodicidad_no_es_reconocida()
    {
        var dto = Valid();
        dto.Period = "annual";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Period);
    }

    [Fact]
    public void Falla_si_no_se_indica_cuando_arranca_el_ciclo()
    {
        var dto = Valid();
        dto.AnchorDate = default;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.AnchorDate);
    }
}
