using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class BillingCycleTests
{
    [Fact]
    public void GetNextOccurrence_devuelve_el_mismo_mes_si_el_dia_no_paso_todavia()
    {
        var result = BillingCycle.GetNextOccurrence(new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc), 20);
        Assert.Equal(new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void GetNextOccurrence_pasa_al_mes_siguiente_si_el_dia_ya_paso()
    {
        var result = BillingCycle.GetNextOccurrence(new DateTime(2026, 8, 25, 0, 0, 0, DateTimeKind.Utc), 20);
        Assert.Equal(new DateTime(2026, 9, 20, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void GetNextOccurrence_devuelve_hoy_si_el_dia_es_hoy()
    {
        var result = BillingCycle.GetNextOccurrence(new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc), 20);
        Assert.Equal(new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void GetNextOccurrence_usa_el_ultimo_dia_del_mes_si_el_dia_no_existe()
    {
        // Día 31 no existe en febrero (2026 no es bisiesto) -> usa el 28.
        var result = BillingCycle.GetNextOccurrence(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), 31);
        Assert.Equal(new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void GetLastOccurrenceOnOrBefore_devuelve_el_mismo_mes_si_el_dia_ya_paso()
    {
        var result = BillingCycle.GetLastOccurrenceOnOrBefore(new DateTime(2026, 8, 25, 0, 0, 0, DateTimeKind.Utc), 20);
        Assert.Equal(new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void GetLastOccurrenceOnOrBefore_retrocede_al_mes_anterior_si_el_dia_no_ha_pasado()
    {
        var result = BillingCycle.GetLastOccurrenceOnOrBefore(new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc), 20);
        Assert.Equal(new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void GetLastOccurrenceOnOrBefore_devuelve_hoy_si_el_dia_es_hoy()
    {
        var result = BillingCycle.GetLastOccurrenceOnOrBefore(new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc), 20);
        Assert.Equal(new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc), result);
    }
}
