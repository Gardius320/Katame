using KatameApi.Models;

namespace KatameApi.Repositories;

public interface ITrainingDayRepository
{
    Task<List<TrainingDay>> GetAllAsync();
    Task<TrainingDay?> GetByIdAsync(int id);
    Task AddAsync(TrainingDay day);
    void Remove(TrainingDay day);
    Task SaveChangesAsync();
}
