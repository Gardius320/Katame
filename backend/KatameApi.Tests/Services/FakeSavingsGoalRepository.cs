using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeSavingsGoalRepository : ISavingsGoalRepository
{
    private readonly List<SavingsGoal> _goals = new();
    private int _nextId = 1;

    public Task<List<SavingsGoal>> GetAllAsync() => Task.FromResult(_goals.ToList());

    public Task<SavingsGoal?> GetByIdAsync(int id) =>
        Task.FromResult(_goals.FirstOrDefault(g => g.Id == id));

    public Task AddAsync(SavingsGoal goal)
    {
        goal.Id = _nextId++;
        _goals.Add(goal);
        return Task.CompletedTask;
    }

    public void Remove(SavingsGoal goal) => _goals.Remove(goal);

    public Task SaveChangesAsync() => Task.CompletedTask;
}
