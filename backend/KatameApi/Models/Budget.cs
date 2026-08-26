namespace KatameApi.Models;

public class Budget : BaseEntity, IUserOwned
{
    public int UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Period { get; set; } = BudgetPeriod.Monthly;

    /// <summary>
    /// Fecha de referencia que define el ciclo del presupuesto: para "monthly"
    /// se usa el día del mes, para "weekly" el día de la semana, y para
    /// "biweekly" sirve de ancla para contar bloques de 14 días. Es la misma
    /// idea que StatementDay en CreditCard, generalizada a distintas
    /// periodicidades (ver BudgetCycle).
    /// </summary>
    public DateTime AnchorDate { get; set; }
}

public static class BudgetPeriod
{
    public const string Weekly = "weekly";
    public const string Biweekly = "biweekly";
    public const string Monthly = "monthly";

    public static readonly string[] All = { Weekly, Biweekly, Monthly };
}
