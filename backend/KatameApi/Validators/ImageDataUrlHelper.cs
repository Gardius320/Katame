namespace KatameApi.Validators;

/// <summary>
/// Valida que un data URL contenga bytes reales de una imagen PNG o JPEG, en vez
/// de confiar solo en el prefijo "data:image/..." (que cualquiera puede falsificar,
/// por ejemplo enviando un SVG con &lt;script&gt; directamente a la API sin pasar
/// por el formulario, que siempre reescribe la imagen a PNG real en el navegador).
/// </summary>
public static class ImageDataUrlHelper
{
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };

    public static bool IsValidImageDataUrl(string? value)
    {
        if (value is null)
        {
            return true;
        }

        var commaIndex = value.IndexOf(',');
        if (!value.StartsWith("data:image/", StringComparison.Ordinal) || commaIndex < 0)
        {
            return false;
        }

        var header = value[..commaIndex];
        if (!header.Contains(";base64", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(value[(commaIndex + 1)..]);
        }
        catch (FormatException)
        {
            return false;
        }

        return StartsWith(bytes, PngSignature) || StartsWith(bytes, JpegSignature);
    }

    private static bool StartsWith(byte[] bytes, byte[] signature)
    {
        if (bytes.Length < signature.Length)
        {
            return false;
        }

        for (var i = 0; i < signature.Length; i++)
        {
            if (bytes[i] != signature[i])
            {
                return false;
            }
        }

        return true;
    }
}
