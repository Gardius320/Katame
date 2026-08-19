namespace KatameApi.Validators;

public static class ColombianDocumentId
{
    /// <summary>
    /// Valida una cédula colombiana: solo dígitos, entre 6 y 10 caracteres,
    /// sin cero a la izquierda. A diferencia de la cédula ecuatoriana,
    /// Colombia no usa un algoritmo de dígito verificador ni código de
    /// provincia: la cédula de ciudadanía es un número secuencial asignado
    /// por la Registraduría Nacional, así que la validación se limita a
    /// formato y longitud.
    /// </summary>
    public static bool IsValidCedula(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.All(char.IsDigit))
        {
            return false;
        }

        if (value.Length < 6 || value.Length > 10)
        {
            return false;
        }

        if (value[0] == '0')
        {
            return false;
        }

        return true;
    }
}
