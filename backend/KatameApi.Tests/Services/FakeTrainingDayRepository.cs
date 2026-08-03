using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeTrainingDayRepository : ITrainingDayRepository
{
    private readonly List<TrainingDay> _days = new();
    private int _nextDayId = 1;
    private int _nextExerciseId = 1;

    public Task<List<TrainingDay>> GetAllAsync() => Task.FromResult(_days.ToList());

    public Task<TrainingDay?> GetByIdAsync(int id) =>
        Task.FromResult(_days.FirstOrDefault(d => d.Id == id));

    public Task AddAsync(TrainingDay day)
    {
        day.Id = _nextDayId++;
        _days.Add(day);
        return Task.CompletedTask;
    }

    public void Remove(TrainingDay day) => _days.Remove(day);

    public Task SaveChangesAsync()
    {
        foreach (var day in _days)
        {
            foreach (var exercise in day.Exercises.Where(e => e.Id == 0))
            {
                exercise.Id = _nextExerciseId++;
            }
        }

        return Task.CompletedTask;
    }
}
