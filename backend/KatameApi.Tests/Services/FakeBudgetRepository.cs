using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeBudgetRepository : IBudgetRepository
{
    private readonly List<Budget> _budgets = new();
    private int _nextId = 1;

    public Task<List<Budget>> GetAllAsync() => Task.FromResult(_budgets.ToList());

    public Task<Budget?> GetByIdAsync(int id) =>
        Task.FromResult(_budgets.FirstOrDefault(b => b.Id == id));

    public Task AddAsync(Budget budget)
    {
        budget.Id = _nextId++;
        _budgets.Add(budget);
        return Task.CompletedTask;
    }

    public void Remove(Budget budget) => _budgets.Remove(budget);

    public Task SaveChangesAsync() => Task.CompletedTask;
}
