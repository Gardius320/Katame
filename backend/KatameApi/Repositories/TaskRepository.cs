using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly KatameDbContext _context;

    public TaskRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<TaskItem>> GetAllAsync() =>
        _context.Tasks.OrderBy(t => t.Date).ThenBy(t => t.Id).ToListAsync();

    public Task<TaskItem?> GetByIdAsync(int id) =>
        _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(TaskItem task) =>
        await _context.Tasks.AddAsync(task);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
