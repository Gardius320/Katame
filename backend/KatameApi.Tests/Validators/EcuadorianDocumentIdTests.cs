using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class EcuadorianDocumentIdTests
{
    [Theory]
    [InlineData("1701234567")]
    [InlineData("1712345675")]
    [InlineData("1723456784")]
    [InlineData("0918273640")]
    public void IsValidCedula_devuelve_true_para_cedulas_con_digito_verificador_correcto(string cedula)
    {
        Assert.True(EcuadorianDocumentId.IsValidCedula(cedula));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123456789")] // 9 dígitos
    [InlineData("12345678901")] // 11 dígitos
    [InlineData("170123456A")] // no numérica
    [InlineData("1234567890")] // dígito verificador incorrecto
    [InlineData("9901234567")] // provincia inválida (99)
    [InlineData("1791234567")] // tercer dígito de persona natural inválido (9)
    public void IsValidCedula_devuelve_false_para_cedulas_invalidas(string? cedula)
    {
        Assert.False(EcuadorianDocumentId.IsValidCedula(cedula!));
    }
}
