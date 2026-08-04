using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeSubscriptionRepository : ISubscriptionRepository
{
    private readonly List<Subscription> _subscriptions = new();
    private int _nextId = 1;

    public Task<List<Subscription>> GetAllAsync() =>
        Task.FromResult(_subscriptions.Where(s => !s.IsDeleted).ToList());

    public Task<Subscription?> GetByIdAsync(int id) =>
        Task.FromResult(_subscriptions.FirstOrDefault(s => s.Id == id && !s.IsDeleted));

    public Task AddAsync(Subscription subscription)
    {
        subscription.Id = _nextId++;
        _subscriptions.Add(subscription);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
