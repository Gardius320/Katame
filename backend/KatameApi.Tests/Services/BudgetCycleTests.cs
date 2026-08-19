using KatameApi.Models;
using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class BudgetCycleTests
{
    [Fact]
    public void Monthly_ciclo_vigente_cuando_el_dia_de_anclaje_ya_paso_este_mes()
    {
        var anchor = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc);
        var today = new DateTime(2026, 8, 25, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = BudgetCycle.GetCurrentCycle(today, BudgetPeriod.Monthly, anchor);

        Assert.Equal(new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 9, 5, 0, 0, 0, DateTimeKind.Utc), end);
    }

    [Fact]
    public void Monthly_ciclo_vigente_cuando_el_dia_de_anclaje_todavia_no_llega_este_mes()
    {
        var anchor = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc);
        var today = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = BudgetCycle.GetCurrentCycle(today, BudgetPeriod.Monthly, anchor);

        Assert.Equal(new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc), end);
    }

    [Fact]
    public void Weekly_ciclo_vigente_arranca_en_el_dia_de_la_semana_del_anclaje()
    {
        // 2026-08-10 es lunes.
        var anchor = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);
        // 2026-08-27 es jueves de la semana siguiente.
        var today = new DateTime(2026, 8, 27, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = BudgetCycle.GetCurrentCycle(today, BudgetPeriod.Weekly, anchor);

        Assert.Equal(new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 8, 31, 0, 0, 0, DateTimeKind.Utc), end);
    }

    [Fact]
    public void Weekly_ciclo_vigente_cuando_hoy_es_el_mismo_dia_de_la_semana_del_anclaje()
    {
        var anchor = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);
        var today = new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = BudgetCycle.GetCurrentCycle(today, BudgetPeriod.Weekly, anchor);

        Assert.Equal(new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 8, 31, 0, 0, 0, DateTimeKind.Utc), end);
    }

    [Fact]
    public void Biweekly_ciclo_vigente_avanza_en_bloques_de_14_dias_desde_el_anclaje()
    {
        var anchor = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);
        var today = new DateTime(2026, 8, 27, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = BudgetCycle.GetCurrentCycle(today, BudgetPeriod.Biweekly, anchor);

        Assert.Equal(new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 9, 7, 0, 0, 0, DateTimeKind.Utc), end);
    }

    [Fact]
    public void Biweekly_ciclo_vigente_cuando_hoy_es_el_dia_de_anclaje()
    {
        var anchor = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = BudgetCycle.GetCurrentCycle(anchor, BudgetPeriod.Biweekly, anchor);

        Assert.Equal(new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc), end);
    }
}
