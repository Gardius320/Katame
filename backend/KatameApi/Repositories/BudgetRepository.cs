using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly KatameDbContext _context;

    public BudgetRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<Budget>> GetAllAsync() =>
        _context.Budgets.OrderBy(b => b.Category).ToListAsync();

    public Task<Budget?> GetByIdAsync(int id) =>
        _context.Budgets.FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Budget budget) =>
        await _context.Budgets.AddAsync(budget);

    public void Remove(Budget budget) =>
        _context.Budgets.Remove(budget);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
