using KatameApi.Models;

namespace KatameApi.Repositories;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(int id);
    Task AddAsync(Subscription subscription);
    Task SaveChangesAsync();
}
