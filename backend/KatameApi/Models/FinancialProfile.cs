namespace KatameApi.Models;

/// <summary>
/// Configuración financiera personal del usuario, fuera de cualquier meta puntual.
/// Por ahora solo guarda el ingreso mensual, usado para calcular qué porcentaje de
/// su ingreso representa el ahorro mensual planeado de cada meta de ahorro. Hay una
/// sola fila por usuario (no una lista) -- se crea la primera vez que la actualiza.
/// </summary>
public class FinancialProfile : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal MonthlyIncome { get; set; }
}
