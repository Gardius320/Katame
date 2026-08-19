using KatameApi.Models;

namespace KatameApi.Services;

/// <summary>
/// Cálculo del ciclo vigente (inicio inclusive / fin exclusivo) de un
/// presupuesto según su periodicidad. El caso mensual reutiliza la lógica de
/// BillingCycle (mismo problema que el corte de una tarjeta: un "día del mes"
/// que hay que clampear en meses más cortos).
/// </summary>
public static class BudgetCycle
{
    public static (DateTime Start, DateTime End) GetCurrentCycle(DateTime today, string period, DateTime anchorDate)
    {
        return period switch
        {
            BudgetPeriod.Weekly => GetWeeklyCycle(today, anchorDate),
            BudgetPeriod.Biweekly => GetBiweeklyCycle(today, anchorDate),
            _ => GetMonthlyCycle(today, anchorDate),
        };
    }

    private static (DateTime Start, DateTime End) GetMonthlyCycle(DateTime today, DateTime anchorDate)
    {
        var start = BillingCycle.GetLastOccurrenceOnOrBefore(today, anchorDate.Day);
        var end = BillingCycle.GetNextOccurrence(start.AddDays(1), anchorDate.Day);
        return (start, end);
    }

    private static (DateTime Start, DateTime End) GetWeeklyCycle(DateTime today, DateTime anchorDate)
    {
        var diff = ((int)today.DayOfWeek - (int)anchorDate.DayOfWeek + 7) % 7;
        var start = today.Date.AddDays(-diff);
        return (start, start.AddDays(7));
    }

    private static (DateTime Start, DateTime End) GetBiweeklyCycle(DateTime today, DateTime anchorDate)
    {
        var daysSinceAnchor = (today.Date - anchorDate.Date).Days;
        var cyclesElapsed = (int)Math.Floor(daysSinceAnchor / 14.0);
        var start = anchorDate.Date.AddDays(cyclesElapsed * 14);
        return (start, start.AddDays(14));
    }
}
