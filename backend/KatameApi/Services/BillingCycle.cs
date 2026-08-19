namespace KatameApi.Services;

/// <summary>
/// Cálculo de fechas de ciclo mensual (día de corte / día de pago de una
/// tarjeta de crédito) a partir de un "día del mes". Si ese día no existe en
/// el mes en cuestión (ej. día 31 en febrero), se usa el último día
/// disponible del mes.
/// </summary>
public static class BillingCycle
{
    /// <summary>
    /// Próxima ocurrencia de <paramref name="dayOfMonth"/> a partir de
    /// <paramref name="referenceDate"/> (inclusive). Se usa para saber cuándo
    /// vence el próximo pago.
    /// </summary>
    public static DateTime GetNextOccurrence(DateTime referenceDate, int dayOfMonth)
    {
        var daysInCurrentMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
        var clampedDay = Math.Min(dayOfMonth, daysInCurrentMonth);
        var candidate = new DateTime(referenceDate.Year, referenceDate.Month, clampedDay, 0, 0, 0, DateTimeKind.Utc);

        if (candidate.Date >= referenceDate.Date)
        {
            return candidate;
        }

        var nextMonth = referenceDate.AddMonths(1);
        var daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
        var clampedNextDay = Math.Min(dayOfMonth, daysInNextMonth);
        return new DateTime(nextMonth.Year, nextMonth.Month, clampedNextDay, 0, 0, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// Última ocurrencia de <paramref name="dayOfMonth"/> hasta
    /// <paramref name="referenceDate"/> (inclusive). Se usa para saber cuándo
    /// fue el último corte de una tarjeta.
    /// </summary>
    public static DateTime GetLastOccurrenceOnOrBefore(DateTime referenceDate, int dayOfMonth)
    {
        var daysInCurrentMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
        var clampedDay = Math.Min(dayOfMonth, daysInCurrentMonth);
        var candidate = new DateTime(referenceDate.Year, referenceDate.Month, clampedDay, 0, 0, 0, DateTimeKind.Utc);

        if (candidate.Date <= referenceDate.Date)
        {
            return candidate;
        }

        var previousMonth = referenceDate.AddMonths(-1);
        var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
        var clampedPreviousDay = Math.Min(dayOfMonth, daysInPreviousMonth);
        return new DateTime(previousMonth.Year, previousMonth.Month, clampedPreviousDay, 0, 0, 0, DateTimeKind.Utc);
    }
}
