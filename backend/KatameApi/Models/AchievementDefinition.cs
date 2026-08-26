namespace KatameApi.Models;

// Metadata fija de un logro posible: no depende del usuario ni se guarda en
// la base de datos. Solo UserAchievement (qué se desbloqueó) vive en la BD.
public record AchievementDefinition(string Key, string Category, string Title, string Description);

public static class AchievementCategory
{
    public const string Finance = "finanzas";
    public const string Training = "entrenamiento";
}

// Catálogo fijo de logros. Agregar uno nuevo es solo sumar una línea acá --
// AchievementService.ComputeMetKeysAsync es el que decide cuándo se cumple
// cada uno.
public static class AchievementCatalog
{
    public static readonly IReadOnlyList<AchievementDefinition> All = new List<AchievementDefinition>
    {
        new(
            "primera_meta_cumplida",
            AchievementCategory.Finance,
            "Primera meta cumplida",
            "Llevaste una meta de ahorro al 100% de su monto objetivo."),
        new(
            "racha_ahorro_3",
            AchievementCategory.Finance,
            "3 meses ahorrando sin parar",
            "Aportaste a una misma meta de ahorro 3 meses seguidos."),
        new(
            "racha_ahorro_6",
            AchievementCategory.Finance,
            "6 meses ahorrando sin parar",
            "Aportaste a una misma meta de ahorro 6 meses seguidos."),
        new(
            "mes_sin_gastos_hormiga",
            AchievementCategory.Finance,
            "Mes sin gastos hormiga",
            "Cerraste un mes completo sin categorías de gasto pequeño y frecuente."),
        new(
            "primer_entrenamiento",
            AchievementCategory.Training,
            "Primer entrenamiento registrado",
            "Marcaste tu primer día de entrenamiento como completado."),
        new(
            "racha_entrenamiento_7",
            AchievementCategory.Training,
            "Racha de 7 días",
            "Entrenaste 7 días planeados seguidos."),
        new(
            "racha_entrenamiento_30",
            AchievementCategory.Training,
            "Racha de 30 días",
            "Entrenaste 30 días planeados seguidos."),
        new(
            "veinticinco_entrenamientos",
            AchievementCategory.Training,
            "25 entrenamientos completados",
            "Acumulaste 25 días de entrenamiento marcados como hechos."),
    };
}
