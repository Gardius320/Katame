using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeGoalRepository : IGoalRepository
{
    private readonly List<Goal> _goals = new();
    private int _nextId = 1;

    public Task<List<Goal>> GetAllAsync() => Task.FromResult(_goals.ToList());

    public Task<Goal?> GetByIdAsync(int id) =>
        Task.FromResult(_goals.FirstOrDefault(g => g.Id == id));

    public Task AddAsync(Goal goal)
    {
        goal.Id = _nextId++;
        _goals.Add(goal);
        return Task.CompletedTask;
    }

    public void Remove(Goal goal) => _goals.Remove(goal);

    public Task SaveChangesAsync() => Task.CompletedTask;
}
