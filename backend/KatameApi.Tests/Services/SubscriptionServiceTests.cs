using AutoMapper;
using KatameApi.DTOs.Subscriptions;
using KatameApi.Middleware;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class SubscriptionServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SubscriptionMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    [Fact]
    public async Task SubscriptionService_crea_actualiza_y_elimina()
    {
        var service = new SubscriptionService(new FakeSubscriptionRepository(), CreateMapper());

        var subscription = await service.CreateAsync(new CreateSubscriptionDto
        {
            Name = "Netflix",
            Amount = 15,
            RenewalDate = DateTime.UtcNow,
            ReminderEnabled = true,
        });

        Assert.True(subscription.ReminderEnabled);

        var updated = await service.UpdateAsync(subscription.Id, new UpdateSubscriptionDto
        {
            Name = "Netflix Premium",
            Amount = 20,
            RenewalDate = subscription.RenewalDate,
            ReminderEnabled = false,
        });

        Assert.Equal("Netflix Premium", updated.Name);
        Assert.Equal(20, updated.Amount);
        Assert.False(updated.ReminderEnabled);

        await service.DeleteAsync(subscription.Id);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task SubscriptionService_DeleteAsync_lanza_ApiException_404_si_no_existe()
    {
        var service = new SubscriptionService(new FakeSubscriptionRepository(), CreateMapper());

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }
}
