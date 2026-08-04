using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IGoalRepository
{
    Task<List<Goal>> GetAllAsync();
    Task<Goal?> GetByIdAsync(int id);
    Task AddAsync(Goal goal);
    void Remove(Goal goal);
    Task SaveChangesAsync();
}
