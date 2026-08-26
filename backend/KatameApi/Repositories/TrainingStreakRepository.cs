using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class TrainingStreakRepository : ITrainingStreakRepository
{
    private readonly KatameDbContext _context;

    public TrainingStreakRepository(KatameDbContext context)
    {
        _context = context;
    }

    private Task<TrainingStreak?> GetAsync() =>
        _context.TrainingStreaks.FirstOrDefaultAsync();

    public async Task<int> GetLongestAsync() =>
        (await GetAsync())?.LongestStreakDays ?? 0;

    public async Task<int> UpdateLongestIfHigherAsync(int candidate)
    {
        var streak = await GetAsync();

        if (streak is not null)
        {
            streak.LongestStreakDays = Math.Max(streak.LongestStreakDays, candidate);
            await _context.SaveChangesAsync();
            return streak.LongestStreakDays;
        }

        var newStreak = new TrainingStreak { LongestStreakDays = candidate };
        await _context.TrainingStreaks.AddAsync(newStreak);

        try
        {
            await _context.SaveChangesAsync();
            return newStreak.LongestStreakDays;
        }
        catch (DbUpdateException)
        {
            // Misma condición de carrera que en FinancialProfileRepository: dos
            // peticiones casi simultáneas intentando crear la fila por primera vez.
            _context.Entry(newStreak).State = EntityState.Detached;

            var winner = await GetAsync();
            if (winner is null)
            {
                throw;
            }

            winner.LongestStreakDays = Math.Max(winner.LongestStreakDays, candidate);
            await _context.SaveChangesAsync();
            return winner.LongestStreakDays;
        }
    }
}
