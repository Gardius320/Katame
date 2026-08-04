using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeProjectRepository : IProjectRepository
{
    private readonly List<Project> _projects = new();
    private int _nextId = 1;

    public Task<List<Project>> GetAllAsync() =>
        Task.FromResult(_projects.Where(p => !p.IsDeleted).ToList());

    public Task<Project?> GetByIdAsync(int id) =>
        Task.FromResult(_projects.FirstOrDefault(p => p.Id == id && !p.IsDeleted));

    public Task AddAsync(Project project)
    {
        project.Id = _nextId++;
        _projects.Add(project);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
