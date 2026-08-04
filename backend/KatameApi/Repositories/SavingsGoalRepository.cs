using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class SavingsGoalRepository : ISavingsGoalRepository
{
    private readonly KatameDbContext _context;

    public SavingsGoalRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<SavingsGoal>> GetAllAsync() =>
        _context.SavingsGoals.OrderBy(g => g.DueDate).ThenBy(g => g.Name).ToListAsync();

    public Task<SavingsGoal?> GetByIdAsync(int id) =>
        _context.SavingsGoals.FirstOrDefaultAsync(g => g.Id == id);

    public async Task AddAsync(SavingsGoal goal) =>
        await _context.SavingsGoals.AddAsync(goal);

    public void Remove(SavingsGoal goal) =>
        _context.SavingsGoals.Remove(goal);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
