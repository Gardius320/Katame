using FluentValidation.TestHelper;
using KatameApi.DTOs.Goals;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateGoalValidatorTests
{
    [Fact]
    public void CreateGoalValidator_falla_si_el_titulo_esta_vacio()
    {
        var result = new CreateGoalValidator().TestValidate(
            new CreateGoalDto { Title = "", Category = "Salud", ProgressPercentage = 0 });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void CreateGoalValidator_falla_si_el_progreso_esta_fuera_de_rango(int progress)
    {
        var result = new CreateGoalValidator().TestValidate(
            new CreateGoalDto { Title = "Correr 10K", Category = "Salud", ProgressPercentage = progress });
        result.ShouldHaveValidationErrorFor(x => x.ProgressPercentage);
    }
}
