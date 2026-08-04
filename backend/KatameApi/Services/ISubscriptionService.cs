using KatameApi.DTOs.Subscriptions;

namespace KatameApi.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionDto>> GetAllAsync();
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto request);
    Task<SubscriptionDto> UpdateAsync(int id, UpdateSubscriptionDto request);
    Task DeleteAsync(int id);
}
