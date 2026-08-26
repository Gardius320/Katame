using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Middleware;
using KatameApi.Models;
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
    public async Task SavingsGoalService_ContributeAsync_suma_al_monto_actual()
    {
        var service = new SavingsGoalService(new FakeSavingsGoalRepository(), CreateMapper());

        var goal = await service.CreateAsync(new CreateSavingsGoalDto
        {
            Name = "Carro",
            TargetAmount = 28_000_000,
            CurrentAmount = 8_000_000,
        });

        var contributed = await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto
        {
            Amount = 3_000_000,
        });

        Assert.Equal(11_000_000, contributed.CurrentAmount);
        Assert.Equal(28_000_000, contributed.TargetAmount);
    }

    [Fact]
    public async Task SavingsGoalService_ContributeAsync_el_primer_aporte_inicia_la_racha_en_1()
    {
        var service = new SavingsGoalService(new FakeSavingsGoalRepository(), CreateMapper());
        var goal = await service.CreateAsync(new CreateSavingsGoalDto { Name = "Carro", TargetAmount = 1000, CurrentAmount = 0 });

        var contributed = await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto { Amount = 100 });

        Assert.Equal(1, contributed.CurrentStreakMonths);
        Assert.Equal(1, contributed.LongestStreakMonths);
    }

    [Fact]
    public async Task SavingsGoalService_ContributeAsync_dos_aportes_el_mismo_mes_no_duplican_la_racha()
    {
        var service = new SavingsGoalService(new FakeSavingsGoalRepository(), CreateMapper());
        var goal = await service.CreateAsync(new CreateSavingsGoalDto { Name = "Carro", TargetAmount = 1000, CurrentAmount = 0 });

        await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto { Amount = 100 });
        var second = await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto { Amount = 50 });

        Assert.Equal(1, second.CurrentStreakMonths);
    }

    [Fact]
    public async Task SavingsGoalService_ContributeAsync_mes_consecutivo_sube_la_racha()
    {
        var repository = new FakeSavingsGoalRepository();
        var service = new SavingsGoalService(repository, CreateMapper());
        var goal = await service.CreateAsync(new CreateSavingsGoalDto { Name = "Carro", TargetAmount = 1000, CurrentAmount = 0 });
        await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto { Amount = 100 });

        // Simula que ese primer aporte fue el mes pasado, para forzar la rama
        // "mes consecutivo" sin depender de esperar a que cambie el mes real.
        var tracked = await repository.GetByIdAsync(goal.Id);
        var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        tracked!.LastContributionMonth = thisMonth.AddMonths(-1);

        var contributed = await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto { Amount = 100 });

        Assert.Equal(2, contributed.CurrentStreakMonths);
        Assert.Equal(2, contributed.LongestStreakMonths);
    }

    [Fact]
    public async Task SavingsGoalService_ContributeAsync_un_mes_saltado_reinicia_la_racha_pero_no_el_record()
    {
        var repository = new FakeSavingsGoalRepository();
        var service = new SavingsGoalService(repository, CreateMapper());
        var goal = await service.CreateAsync(new CreateSavingsGoalDto { Name = "Carro", TargetAmount = 1000, CurrentAmount = 0 });

        var tracked = await repository.GetByIdAsync(goal.Id);
        tracked!.CurrentStreakMonths = 5;
        tracked.LongestStreakMonths = 5;
        var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        tracked.LastContributionMonth = thisMonth.AddMonths(-3); // se saltó dos meses

        var contributed = await service.ContributeAsync(goal.Id, new ContributeSavingsGoalDto { Amount = 100 });

        Assert.Equal(1, contributed.CurrentStreakMonths);
        Assert.Equal(5, contributed.LongestStreakMonths); // el récord no se pierde
    }

    [Fact]
    public async Task SavingsGoalService_ContributeAsync_lanza_ApiException_404_si_no_existe()
    {
        var service = new SavingsGoalService(new FakeSavingsGoalRepository(), CreateMapper());

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.ContributeAsync(999, new ContributeSavingsGoalDto
        {
            Amount = 1000,
        }));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task SavingsGoalService_guarda_el_ahorro_mensual_planeado()
    {
        var service = new SavingsGoalService(new FakeSavingsGoalRepository(), CreateMapper());

        var goal = await service.CreateAsync(new CreateSavingsGoalDto
        {
            Name = "Carro",
            TargetAmount = 28_000_000,
            CurrentAmount = 8_000_000,
            MonthlyContributionTarget = 400_000,
        });

        Assert.Equal(400_000, goal.MonthlyContributionTarget);

        var updated = await service.UpdateAsync(goal.Id, new UpdateSavingsGoalDto
        {
            Name = "Carro",
            TargetAmount = 28_000_000,
            CurrentAmount = 8_000_000,
            MonthlyContributionTarget = null,
        });

        Assert.Null(updated.MonthlyContributionTarget);
    }

    [Fact]
    public async Task FinancialProfileService_GetAsync_devuelve_cero_si_no_se_ha_configurado()
    {
        var service = new FinancialProfileService(new FakeFinancialProfileRepository(), CreateMapper());

        var profile = await service.GetAsync();

        Assert.Equal(0, profile.MonthlyIncome);
    }

    [Fact]
    public async Task FinancialProfileService_UpdateAsync_crea_y_luego_actualiza_la_misma_fila()
    {
        var service = new FinancialProfileService(new FakeFinancialProfileRepository(), CreateMapper());

        var created = await service.UpdateAsync(new UpdateFinancialProfileDto { MonthlyIncome = 5_000_000 });
        Assert.Equal(5_000_000, created.MonthlyIncome);

        var updated = await service.UpdateAsync(new UpdateFinancialProfileDto { MonthlyIncome = 5_500_000 });
        Assert.Equal(5_500_000, updated.MonthlyIncome);

        var fetched = await service.GetAsync();
        Assert.Equal(5_500_000, fetched.MonthlyIncome);
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
    public async Task ObligationService_guarda_la_frecuencia_cuando_es_recurrente()
    {
        var service = new ObligationService(new FakeObligationRepository(), CreateMapper());

        var obligation = await service.CreateAsync(new CreateObligationDto
        {
            Name = "Arriendo",
            Amount = 1_200_000,
            DueDate = DateTime.UtcNow,
            IsRecurring = true,
            RecurrenceFrequency = RecurrenceFrequency.Monthly,
        });

        Assert.Equal(RecurrenceFrequency.Monthly, obligation.RecurrenceFrequency);

        var updated = await service.UpdateAsync(obligation.Id, new UpdateObligationDto
        {
            Name = "Arriendo",
            Amount = 1_200_000,
            DueDate = obligation.DueDate,
            IsRecurring = true,
            RecurrenceFrequency = RecurrenceFrequency.Biweekly,
            IsPaid = false,
        });

        Assert.Equal(RecurrenceFrequency.Biweekly, updated.RecurrenceFrequency);
    }

    [Fact]
    public async Task ObligationService_ignora_la_frecuencia_cuando_no_es_recurrente()
    {
        var service = new ObligationService(new FakeObligationRepository(), CreateMapper());

        var obligation = await service.CreateAsync(new CreateObligationDto
        {
            Name = "Internet",
            Amount = 90_000,
            DueDate = DateTime.UtcNow,
            IsRecurring = false,
            RecurrenceFrequency = RecurrenceFrequency.Monthly,
        });

        Assert.Null(obligation.RecurrenceFrequency);
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
        var service = new CreditCardService(new FakeCreditCardRepository(), new FakeTransactionRepository(), CreateMapper());

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
