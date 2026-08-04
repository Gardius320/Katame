using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly KatameDbContext _context;

    public GoalRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<Goal>> GetAllAsync() =>
        _context.Goals.OrderBy(g => g.DueDate).ThenBy(g => g.Title).ToListAsync();

    public Task<Goal?> GetByIdAsync(int id) =>
        _context.Goals.FirstOrDefaultAsync(g => g.Id == id);

    public async Task AddAsync(Goal goal) =>
        await _context.Goals.AddAsync(goal);

    public void Remove(Goal goal) =>
        _context.Goals.Remove(goal);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
