using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Models;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class BudgetServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FinanceMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private static BudgetService CreateService(
        FakeBudgetRepository budgets, FakeTransactionRepository transactions) =>
        new(budgets, transactions, CreateMapper());

    [Fact]
    public async Task GetAllAsync_calcula_el_gastado_del_ciclo_vigente_por_categoria()
    {
        var today = DateTime.UtcNow.Date;
        // Mismo criterio que en CreditCardServiceTests: offset de 10 días (no 1) para que el
        // cálculo sea estable sin importar qué día del mes corre el test.
        var anchor = today.AddDays(-10);

        var budgets = new FakeBudgetRepository();
        await budgets.AddAsync(new Budget
        {
            Category = "Comida",
            Amount = 300,
            Period = BudgetPeriod.Monthly,
            AnchorDate = anchor,
        });
        var budget = (await budgets.GetAllAsync())[0];

        var transactions = new FakeTransactionRepository();
        // Dentro del ciclo vigente, misma categoría -> cuenta.
        await transactions.AddAsync(new Transaction { Amount = 100, Type = "expense", Category = "Comida", Date = today.AddDays(-5) });
        await transactions.AddAsync(new Transaction { Amount = 50, Type = "expense", Category = "Comida", Date = today });
        // Antes de que arrancara el ciclo -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 999, Type = "expense", Category = "Comida", Date = today.AddDays(-11) });
        // Otra categoría -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 555, Type = "expense", Category = "Transporte", Date = today.AddDays(-3) });
        // Ingreso (no es gasto) -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 300, Type = "income", Category = "Comida", Date = today.AddDays(-3) });

        var service = CreateService(budgets, transactions);

        var result = await service.GetAllAsync();

        var dto = Assert.Single(result);
        Assert.Equal(150, dto.Spent);
        Assert.Equal(budget.Id, dto.Id);
    }

    [Fact]
    public async Task CreateAsync_guarda_la_categoria_el_monto_y_la_periodicidad()
    {
        var service = CreateService(new FakeBudgetRepository(), new FakeTransactionRepository());

        var created = await service.CreateAsync(new CreateBudgetDto
        {
            Category = "Comida",
            Amount = 500_000,
            Period = BudgetPeriod.Weekly,
            AnchorDate = DateTime.UtcNow.Date,
        });

        Assert.Equal("Comida", created.Category);
        Assert.Equal(500_000, created.Amount);
        Assert.Equal(BudgetPeriod.Weekly, created.Period);
        Assert.Equal(0, created.Spent);
    }

    [Fact]
    public async Task UpdateAsync_permite_cambiar_categoria_monto_y_periodicidad()
    {
        var budgets = new FakeBudgetRepository();
        var service = CreateService(budgets, new FakeTransactionRepository());
        var created = await service.CreateAsync(new CreateBudgetDto
        {
            Category = "Comida",
            Amount = 500_000,
            Period = BudgetPeriod.Monthly,
            AnchorDate = DateTime.UtcNow.Date,
        });

        var updated = await service.UpdateAsync(created.Id, new UpdateBudgetDto
        {
            Category = "Transporte",
            Amount = 200_000,
            Period = BudgetPeriod.Biweekly,
            AnchorDate = DateTime.UtcNow.Date,
        });

        Assert.Equal("Transporte", updated.Category);
        Assert.Equal(200_000, updated.Amount);
        Assert.Equal(BudgetPeriod.Biweekly, updated.Period);
    }

    [Fact]
    public async Task DeleteAsync_elimina_el_presupuesto()
    {
        var budgets = new FakeBudgetRepository();
        var service = CreateService(budgets, new FakeTransactionRepository());
        var created = await service.CreateAsync(new CreateBudgetDto
        {
            Category = "Comida",
            Amount = 500_000,
            Period = BudgetPeriod.Monthly,
            AnchorDate = DateTime.UtcNow.Date,
        });

        await service.DeleteAsync(created.Id);

        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task GetAntExpensesAsync_solo_considera_gastos_del_mes_en_curso()
    {
        var today = DateTime.UtcNow.Date;
        var firstOfThisMonth = new DateTime(today.Year, today.Month, 1);

        var transactions = new FakeTransactionRepository();
        // Dentro del mes en curso: categoría chica y frecuente -> hormiga.
        // Se usa "today" repetido (no offsets de día) para que la fecha caiga
        // dentro de la ventana [inicio de mes, hoy] sin importar qué día del
        // mes sea "hoy" cuando corra el test (ej. si hoy es el día 2, sumarle
        // días al inicio de mes se pasaría de "hoy" y quedaría fuera).
        await transactions.AddAsync(new Transaction { Amount = 5, Type = "expense", Category = "Café", Date = today });
        await transactions.AddAsync(new Transaction { Amount = 5, Type = "expense", Category = "Café", Date = today });
        await transactions.AddAsync(new Transaction { Amount = 5, Type = "expense", Category = "Café", Date = today });
        await transactions.AddAsync(new Transaction { Amount = 5, Type = "expense", Category = "Café", Date = today });
        await transactions.AddAsync(new Transaction { Amount = 500, Type = "expense", Category = "Arriendo", Date = today });
        // Del mes pasado -> no debería contar para esta ventana.
        await transactions.AddAsync(new Transaction { Amount = 5, Type = "expense", Category = "Café", Date = firstOfThisMonth.AddDays(-5) });

        var service = CreateService(new FakeBudgetRepository(), transactions);

        var result = await service.GetAntExpensesAsync();

        var flagged = Assert.Single(result);
        Assert.Equal("Café", flagged.Category);
        Assert.Equal(4, flagged.TransactionCount);
    }
}
