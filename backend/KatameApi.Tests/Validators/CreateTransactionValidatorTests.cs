using FluentValidation.TestHelper;
using KatameApi.DTOs.Finance;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateTransactionValidatorTests
{
    private readonly CreateTransactionValidator _validator = new();

    [Fact]
    public void Falla_cuando_el_monto_es_cero_o_negativo()
    {
        var result = _validator.TestValidate(new CreateTransactionDto { Amount = 0, Type = "income", Category = "Salario" });
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Falla_cuando_el_tipo_no_es_valido()
    {
        var result = _validator.TestValidate(new CreateTransactionDto { Amount = 10, Type = "otro", Category = "Salario" });
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Theory]
    [InlineData("income")]
    [InlineData("expense")]
    public void Pasa_con_datos_validos(string type)
    {
        var result = _validator.TestValidate(new CreateTransactionDto { Amount = 10, Type = type, Category = "Salario" });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
