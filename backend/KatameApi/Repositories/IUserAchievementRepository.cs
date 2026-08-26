using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IUserAchievementRepository
{
    Task<List<UserAchievement>> GetAllAsync();

    /// <summary>
    /// Intenta desbloquear el logro <paramref name="key"/> para el usuario
    /// actual. Devuelve true solo si esta llamada fue la que lo desbloqueó de
    /// verdad -- si ya estaba desbloqueado (o otra petición casi simultánea
    /// ganó la carrera) devuelve false, para que el llamador sepa que NO debe
    /// mostrar la celebración de nuevo.
    /// </summary>
    Task<bool> UnlockAsync(string key);
}
