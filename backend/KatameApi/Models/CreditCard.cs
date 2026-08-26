namespace KatameApi.Models;

public class CreditCard : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StatementDay { get; set; }
    public int PaymentDay { get; set; }
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// Logo del banco como data URL (base64), subido por el usuario al crear
    /// o editar la tarjeta. Opcional -- si no hay logo, el frontend muestra
    /// un ícono genérico de tarjeta.
    /// </summary>
    public string? LogoDataUrl { get; set; }

    /// <summary>
    /// Nombre del banco emisor (ej. "Bancolombia", "Nequi"), elegido de una
    /// lista o escrito libremente. Opcional -- se usa para mostrar una
    /// insignia de color con las iniciales del banco cuando no hay LogoDataUrl.
    /// </summary>
    public string? Bank { get; set; }
}
