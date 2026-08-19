namespace KatameApi.DTOs.Finance;

public class CreditCardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StatementDay { get; set; }
    public int PaymentDay { get; set; }
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// Suma de gastos con esta tarjeta desde el último corte hasta hoy
    /// (ciclo abierto todavía). Se calcula en el service, no se persiste.
    /// </summary>
    public decimal CycleUsage { get; set; }

    /// <summary>Logo del banco como data URL (base64), subido por el usuario.</summary>
    public string? LogoDataUrl { get; set; }
    public string? Bank { get; set; }
}
