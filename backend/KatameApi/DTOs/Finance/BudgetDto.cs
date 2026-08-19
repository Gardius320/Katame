namespace KatameApi.DTOs.Finance;

public class BudgetDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime AnchorDate { get; set; }

    /// <summary>Inicio del ciclo vigente (inclusive).</summary>
    public DateTime CycleStart { get; set; }

    /// <summary>Fecha en la que se reinicia el ciclo (exclusive).</summary>
    public DateTime CycleEnd { get; set; }

    /// <summary>Gastado en la categoría durante el ciclo vigente.</summary>
    public decimal Spent { get; set; }
}
