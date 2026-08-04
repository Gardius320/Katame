using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Subscriptions;

public class SubscriptionMappingProfile : Profile
{
    public SubscriptionMappingProfile()
    {
        CreateMap<Subscription, SubscriptionDto>();
    }
}
