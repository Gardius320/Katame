using KatameApi.DTOs.Achievements;

namespace KatameApi.Services;

public interface IAchievementService
{
    /// <summary>Catálogo completo con el estado (desbloqueado o no) de cada logro.</summary>
    Task<List<AchievementDto>> GetAllAsync();

    /// <summary>
    /// Revisa el catálogo contra el estado actual del usuario y desbloquea lo
    /// que corresponda. Devuelve SOLO los logros que se desbloquearon recién
    /// en esta llamada (para poder celebrarlos); si no hay nada nuevo,
    /// devuelve una lista vacía.
    /// </summary>
    Task<List<AchievementDto>> EvaluateAndUnlockAsync();
}
