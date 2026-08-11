namespace KatameApi.Validators;

public static class EcuadorianDocumentId
{
    /// <summary>
    /// Valida una cédula ecuatoriana: 10 dígitos, código de provincia (01-24),
    /// tercer dígito de persona natural (0-6) y dígito verificador (módulo 10).
    /// </summary>
    public static bool IsValidCedula(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 10 || !value.All(char.IsDigit))
        {
            return false;
        }

        var digits = value.Select(c => c - '0').ToArray();

        var province = digits[0] * 10 + digits[1];
        if (province < 1 || province > 24)
        {
            return false;
        }

        if (digits[2] > 6)
        {
            return false;
        }

        var coefficients = new[] { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            var product = digits[i] * coefficients[i];
            if (product > 9)
            {
                product -= 9;
            }

            sum += product;
        }

        var verifier = (10 - (sum % 10)) % 10;
        return verifier == digits[9];
    }
}
