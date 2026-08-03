using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public Task<List<TaskItem>> GetAllAsync() =>
        Task.FromResult(_tasks.Where(t => !t.IsDeleted).ToList());

    public Task<TaskItem?> GetByIdAsync(int id) =>
        Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id && !t.IsDeleted));

    public Task AddAsync(TaskItem task)
    {
        task.Id = _nextId++;
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
