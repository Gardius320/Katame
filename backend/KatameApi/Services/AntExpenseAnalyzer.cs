using KatameApi.DTOs.Finance;
using KatameApi.Models;

namespace KatameApi.Services;

/// <summary>
/// Detecta "gastos hormiga": categorías con transacciones pequeñas pero
/// frecuentes que, sumadas, se comen una parte importante del gasto sin que
/// se note en una sola transacción.
///
/// El umbral es relativo a los propios hábitos del usuario (no un monto fijo
/// en pesos): se compara el promedio de cada categoría contra el promedio
/// general de gasto del período. Así se autocalibra sin importar el ingreso
/// o la moneda de quien use la app.
/// </summary>
public static class AntExpenseAnalyzer
{
    // Con menos gastos que esto en el período no hay suficiente información
    // para sacar conclusiones -- mejor no mostrar nada que un falso positivo.
    private const int MinTransactionsToEvaluate = 5;

    // Una categoría necesita al menos esta cantidad de movimientos en el
    // período para contar como "frecuente" (más o menos una vez por semana).
    private const int MinFrequency = 4;

    // Una categoría es "pequeña" si su propio promedio está por debajo de la
    // mitad del promedio general de gasto del período.
    private const decimal RelativeThreshold = 0.5m;

    public static List<AntExpenseDto> Analyze(IReadOnlyCollection<Transaction> expenseTransactions)
    {
        if (expenseTransactions.Count < MinTransactionsToEvaluate)
        {
            return new List<AntExpenseDto>();
        }

        var overallAverage = expenseTransactions.Average(t => t.Amount);

        return expenseTransactions
            .GroupBy(t => t.Category)
            .Where(group => group.Count() >= MinFrequency && group.Average(t => t.Amount) < overallAverage * RelativeThreshold)
            .Select(group => new AntExpenseDto
            {
                Category = group.Key,
                TransactionCount = group.Count(),
                TotalAmount = group.Sum(t => t.Amount),
                AverageAmount = group.Average(t => t.Amount),
            })
            .OrderByDescending(a => a.TotalAmount)
            .ToList();
    }
}
