using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IBudgetRepository
{
    Task<List<Budget>> GetAllAsync();
    Task<Budget?> GetByIdAsync(int id);
    Task AddAsync(Budget budget);
    void Remove(Budget budget);
    Task SaveChangesAsync();
}
