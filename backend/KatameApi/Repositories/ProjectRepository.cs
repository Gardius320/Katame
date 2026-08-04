using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly KatameDbContext _context;

    public ProjectRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<Project>> GetAllAsync() =>
        _context.Projects.OrderBy(p => p.Name).ToListAsync();

    public Task<Project?> GetByIdAsync(int id) =>
        _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Project project) =>
        await _context.Projects.AddAsync(project);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
