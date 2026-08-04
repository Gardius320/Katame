using KatameApi.Models;

namespace KatameApi.Repositories;

public interface ISavingsGoalRepository
{
    Task<List<SavingsGoal>> GetAllAsync();
    Task<SavingsGoal?> GetByIdAsync(int id);
    Task AddAsync(SavingsGoal goal);
    void Remove(SavingsGoal goal);
    Task SaveChangesAsync();
}
