using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class TrainingDayRepository : ITrainingDayRepository
{
    private readonly KatameDbContext _context;

    public TrainingDayRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<TrainingDay>> GetAllAsync() =>
        _context.TrainingDays.Include(d => d.Exercises).ToListAsync();

    public Task<TrainingDay?> GetByIdAsync(int id) =>
        _context.TrainingDays.Include(d => d.Exercises).FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAsync(TrainingDay day) =>
        await _context.TrainingDays.AddAsync(day);

    public void Remove(TrainingDay day) =>
        _context.TrainingDays.Remove(day);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
