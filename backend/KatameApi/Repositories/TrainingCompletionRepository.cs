using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class TrainingCompletionRepository : ITrainingCompletionRepository
{
    private readonly KatameDbContext _context;

    public TrainingCompletionRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<DateTime>> GetAllDatesAsync() =>
        _context.TrainingCompletions.Select(c => c.Date).ToListAsync();

    public Task<bool> ExistsForDateAsync(DateTime date) =>
        _context.TrainingCompletions.AnyAsync(c => c.Date == date);

    public async Task AddAsync(TrainingCompletion completion) =>
        await _context.TrainingCompletions.AddAsync(completion);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
