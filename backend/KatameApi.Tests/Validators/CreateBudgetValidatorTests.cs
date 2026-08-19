using FluentValidation.TestHelper;
using KatameApi.DTOs.Finance;
using KatameApi.Models;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateBudgetValidatorTests
{
    private readonly CreateBudgetValidator _validator = new();

    private static CreateBudgetDto Valid() => new()
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
    public void Falla_si_la_categoria_supera_los_50_caracteres()
    {
        var dto = Valid();
        dto.Category = new string('A', 51);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Falla_si_el_monto_no_es_mayor_a_cero()
    {
        var dto = Valid();
        dto.Amount = 0;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(BudgetPeriod.Weekly)]
    [InlineData(BudgetPeriod.Biweekly)]
    [InlineData(BudgetPeriod.Monthly)]
    public void Pasa_con_cualquier_periodicidad_valida(string period)
    {
        var dto = Valid();
        dto.Period = period;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Period);
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
