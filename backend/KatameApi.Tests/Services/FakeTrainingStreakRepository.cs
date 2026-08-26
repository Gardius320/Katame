using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeTrainingStreakRepository : ITrainingStreakRepository
{
    private int _longest;

    public Task<int> GetLongestAsync() => Task.FromResult(_longest);

    public Task<int> UpdateLongestIfHigherAsync(int candidate)
    {
        _longest = Math.Max(_longest, candidate);
        return Task.FromResult(_longest);
    }
}
