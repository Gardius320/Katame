namespace KatameApi.Extensions;

// Las fechas que llegan del frontend (por ejemplo "2026-08-30") no deberían
// desplazarse según la zona horaria del servidor -- el día que la persona eligió
// debe quedar guardado tal cual. `ToUniversalTime()` interpreta una fecha sin
// zona horaria como si fuera "hora local del servidor" y la convierte a UTC
// aplicando ese desfase; hoy eso da el resultado correcto solo porque el
// contenedor donde corre el backend está en UTC por defecto, no porque el
// código lo garantice. Estas extensiones simplemente marcan la fecha como UTC
// sin desplazarla, así el resultado no depende de la zona horaria del entorno.
public static class DateTimeExtensions
{
    public static DateTime AsUtc(this DateTime value) =>
        DateTime.SpecifyKind(value, DateTimeKind.Utc);

    public static DateTime? AsUtc(this DateTime? value) =>
        value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
}
