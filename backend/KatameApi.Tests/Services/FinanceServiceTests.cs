using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Middleware;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class FinanceServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FinanceMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    [Fact]
    public async Task SavingsGoalService_crea_actualiza_y_elimina()
    {
        var service = new SavingsGoalService(new FakeSavingsGoalRepository(), CreateMapper());

        var goal = await service.CreateAsync(new CreateSavingsGoalDto
        {
            Name = "Vacaciones",
            TargetAmount = 2000,
            CurrentAmount = 500,
        });

        var updated = await service.UpdateAsync(goal.Id, new UpdateSavingsGoalDto
        {
            Name = "Vacaciones",
            TargetAmount = 2000,
            CurrentAmount = 800,
        });

        Assert.Equal(800, updated.CurrentAmount);

        await service.DeleteAsync(goal.Id);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task ObligationService_marca_como_pagada()
    {
        var service = new ObligationService(new FakeObligationRepository(), CreateMapper());

        var obligation = await service.CreateAsync(new CreateObligationDto
        {
            Name = "Alquiler",
            Amount = 800,
            DueDate = DateTime.UtcNow,
            IsRecurring = true,
        });

        Assert.False(obligation.IsPaid);

        var updated = await service.UpdateAsync(obligation.Id, new UpdateObligationDto
        {
            Name = "Alquiler",
            Amount = 800,
            DueDate = obligation.DueDate,
            IsRecurring = true,
            IsPaid = true,
        });

        Assert.True(updated.IsPaid);
    }

    [Fact]
    public async Task ObligationService_DeleteAsync_lanza_ApiException_404_si_no_existe()
    {
        var service = new ObligationService(new FakeObligationRepository(), CreateMapper());

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task CreditCardService_crea_y_elimina()
    {
        var service = new CreditCardService(new FakeCreditCardRepository(), CreateMapper());

        var card = await service.CreateAsync(new CreateCreditCardDto
        {
            Name = "Visa Gold",
            StatementDay = 15,
            PaymentDay = 5,
            CreditLimit = 5000,
        });

        Assert.Single(await service.GetAllAsync());

        await service.DeleteAsync(card.Id);
        Assert.Empty(await service.GetAllAsync());
    }
}
