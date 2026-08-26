using KatameApi.Models;

namespace KatameApi.Repositories;

public interface ITrainingCompletionRepository
{
    Task<List<DateTime>> GetAllDatesAsync();
    Task<bool> ExistsForDateAsync(DateTime date);
    Task AddAsync(TrainingCompletion completion);
    Task SaveChangesAsync();
}
