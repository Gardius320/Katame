using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeTrainingCompletionRepository : ITrainingCompletionRepository
{
    private readonly List<TrainingCompletion> _completions = new();
    private int _nextId = 1;

    public Task<List<DateTime>> GetAllDatesAsync() =>
        Task.FromResult(_completions.Select(c => c.Date).ToList());

    public Task<bool> ExistsForDateAsync(DateTime date) =>
        Task.FromResult(_completions.Any(c => c.Date == date));

    public Task AddAsync(TrainingCompletion completion)
    {
        completion.Id = _nextId++;
        _completions.Add(completion);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
