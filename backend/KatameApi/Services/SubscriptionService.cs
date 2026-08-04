using System.Net;
using AutoMapper;
using KatameApi.DTOs.Subscriptions;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper)
    {
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
    }

    public async Task<List<SubscriptionDto>> GetAllAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return _mapper.Map<List<SubscriptionDto>>(subscriptions);
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto request)
    {
        var subscription = new Subscription
        {
            Name = request.Name,
            Amount = request.Amount,
            RenewalDate = request.RenewalDate.ToUniversalTime(),
            ReminderEnabled = request.ReminderEnabled,
        };

        await _subscriptionRepository.AddAsync(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        return _mapper.Map<SubscriptionDto>(subscription);
    }

    public async Task<SubscriptionDto> UpdateAsync(int id, UpdateSubscriptionDto request)
    {
        var subscription = await GetSubscriptionOrThrowAsync(id);

        subscription.Name = request.Name;
        subscription.Amount = request.Amount;
        subscription.RenewalDate = request.RenewalDate.ToUniversalTime();
        subscription.ReminderEnabled = request.ReminderEnabled;

        await _subscriptionRepository.SaveChangesAsync();

        return _mapper.Map<SubscriptionDto>(subscription);
    }

    public async Task DeleteAsync(int id)
    {
        var subscription = await GetSubscriptionOrThrowAsync(id);
        subscription.IsDeleted = true;
        await _subscriptionRepository.SaveChangesAsync();
    }

    private async Task<Subscription> GetSubscriptionOrThrowAsync(int id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription is null)
        {
            throw new ApiException("La suscripción no existe.", HttpStatusCode.NotFound);
        }

        return subscription;
    }
}
