using KatameApi.Models;
using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class AntExpenseAnalyzerTests
{
    private static Transaction Expense(string category, decimal amount) =>
        new() { Type = TransactionType.Expense, Category = category, Amount = amount, Date = DateTime.UtcNow };

    [Fact]
    public void Analyze_detecta_categoria_con_gastos_pequenos_y_frecuentes()
    {
        var transactions = new List<Transaction>
        {
            // "Café": 5 gastos chicos y frecuentes -> hormiga.
            Expense("Café", 5),
            Expense("Café", 4),
            Expense("Café", 6),
            Expense("Café", 5),
            Expense("Café", 5),
            // "Arriendo": un gasto grande, ocasional -> no es hormiga.
            Expense("Arriendo", 500),
        };

        var result = AntExpenseAnalyzer.Analyze(transactions);

        var flagged = Assert.Single(result);
        Assert.Equal("Café", flagged.Category);
        Assert.Equal(5, flagged.TransactionCount);
    }

    [Fact]
    public void Analyze_no_marca_categoria_frecuente_pero_de_monto_similar_al_promedio()
    {
        // Todas las categorías tienen montos parecidos -- ninguna es
        // "chica" respecto al promedio general, así que ninguna se marca.
        var transactions = new List<Transaction>
        {
            Expense("Comida", 50),
            Expense("Comida", 48),
            Expense("Comida", 52),
            Expense("Comida", 49),
            Expense("Transporte", 45),
        };

        var result = AntExpenseAnalyzer.Analyze(transactions);

        Assert.Empty(result);
    }

    [Fact]
    public void Analyze_no_marca_categoria_pequena_pero_poco_frecuente()
    {
        var transactions = new List<Transaction>
        {
            // "Café" solo aparece 2 veces -- no llega al mínimo de frecuencia.
            Expense("Café", 5),
            Expense("Café", 5),
            Expense("Arriendo", 500),
            Expense("Servicios", 300),
            Expense("Mercado", 200),
        };

        var result = AntExpenseAnalyzer.Analyze(transactions);

        Assert.Empty(result);
    }

    [Fact]
    public void Analyze_no_evalua_si_hay_muy_pocas_transacciones_en_el_periodo()
    {
        var transactions = new List<Transaction>
        {
            Expense("Café", 5),
            Expense("Café", 5),
            Expense("Café", 5),
        };

        var result = AntExpenseAnalyzer.Analyze(transactions);

        Assert.Empty(result);
    }
}
