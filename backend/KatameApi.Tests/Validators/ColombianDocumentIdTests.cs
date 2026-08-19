using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class ColombianDocumentIdTests
{
    [Theory]
    [InlineData("1001234567")]
    [InlineData("123456")]
    [InlineData("999999999")]
    [InlineData("1234567890")]
    public void IsValidCedula_devuelve_true_para_cedulas_con_formato_valido(string cedula)
    {
        Assert.True(ColombianDocumentId.IsValidCedula(cedula));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")] // 5 dígitos, muy corta
    [InlineData("12345678901")] // 11 dígitos, muy larga
    [InlineData("170123456A")] // no numérica
    [InlineData("0123456789")] // no puede empezar en 0
    public void IsValidCedula_devuelve_false_para_cedulas_invalidas(string? cedula)
    {
        Assert.False(ColombianDocumentId.IsValidCedula(cedula!));
    }
}
