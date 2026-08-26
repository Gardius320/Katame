namespace KatameApi.Models;

/// <summary>
/// Frecuencia de una obligación recurrente. Solo aplica cuando IsRecurring es true;
/// no se pide una fecha de calendario para las obligaciones recurrentes, solo esta
/// frecuencia (quincenal o mensual).
/// </summary>
public enum RecurrenceFrequency
{
    Biweekly,
    Monthly,
}
