using FluentValidation.TestHelper;
using KatameApi.DTOs.Training;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateTrainingDayValidatorTests
{
    private readonly CreateTrainingDayValidator _validator = new();

    [Fact]
    public void Falla_cuando_el_titulo_esta_vacio()
    {
        var result = _validator.TestValidate(new CreateTrainingDayDto { Title = "", DayOfWeek = DayOfWeek.Monday });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Pasa_con_datos_validos()
    {
        var result = _validator.TestValidate(new CreateTrainingDayDto { Title = "Empuje", DayOfWeek = DayOfWeek.Monday });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
