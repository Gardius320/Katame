namespace KatameApi.DTOs.Finance;

// Una categoría de gasto marcada como "hormiga": transacciones pequeñas y
// frecuentes que, sumadas, representan más de lo que parecen a simple vista.
public class AntExpenseDto
{
    public string Category { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
}
