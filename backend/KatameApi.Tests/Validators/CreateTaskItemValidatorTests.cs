using FluentValidation.TestHelper;
using KatameApi.DTOs.Tasks;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateTaskItemValidatorTests
{
    private readonly CreateTaskItemValidator _validator = new();

    [Fact]
    public void Falla_cuando_el_titulo_esta_vacio()
    {
        var result = _validator.TestValidate(new CreateTaskItemDto { Title = "", Status = "pending" });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Falla_cuando_el_estado_no_es_valido()
    {
        var result = _validator.TestValidate(new CreateTaskItemDto { Title = "Tarea", Status = "invalido" });
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("in_progress")]
    [InlineData("done")]
    public void Pasa_con_datos_validos(string status)
    {
        var result = _validator.TestValidate(new CreateTaskItemDto { Title = "Tarea", Status = status });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
