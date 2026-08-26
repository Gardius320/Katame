using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class UserAchievementRepository : IUserAchievementRepository
{
    private readonly KatameDbContext _context;

    public UserAchievementRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<UserAchievement>> GetAllAsync() =>
        _context.UserAchievements.ToListAsync();

    public async Task<bool> UnlockAsync(string key)
    {
        var alreadyUnlocked = await _context.UserAchievements.AnyAsync(a => a.Key == key);
        if (alreadyUnlocked)
        {
            return false;
        }

        var achievement = new UserAchievement { Key = key, UnlockedAt = DateTime.UtcNow };
        await _context.UserAchievements.AddAsync(achievement);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            // Misma condición de carrera que en FinancialProfileRepository /
            // TrainingStreakRepository: otra petición desbloqueó este mismo
            // logro casi al mismo tiempo. Se destraquea para que no quede una
            // entidad fallida pegada en el ChangeTracker antes de seguir
            // evaluando el resto del catálogo en la misma petición.
            _context.Entry(achievement).State = EntityState.Detached;
            return false;
        }
    }
}
