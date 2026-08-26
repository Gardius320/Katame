using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeUserAchievementRepository : IUserAchievementRepository
{
    private readonly List<UserAchievement> _achievements = new();
    private int _nextId = 1;

    public Task<List<UserAchievement>> GetAllAsync() => Task.FromResult(_achievements.ToList());

    public Task<bool> UnlockAsync(string key)
    {
        if (_achievements.Any(a => a.Key == key))
        {
            return Task.FromResult(false);
        }

        _achievements.Add(new UserAchievement { Id = _nextId++, Key = key, UnlockedAt = DateTime.UtcNow });
        return Task.FromResult(true);
    }
}
