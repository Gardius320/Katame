using AutoMapper;
using KatameApi.DTOs.Tasks;
using KatameApi.DTOs.Today;
using KatameApi.DTOs.Training;
using KatameApi.Models;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class TodayServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TaskMappingProfile>();
            cfg.AddProfile<TrainingMappingProfile>();
        }, NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private static TodayService CreateService(
        FakeTransactionRepository transactions,
        FakeObligationRepository obligations,
        FakeCreditCardRepository creditCards,
        FakeSubscriptionRepository subscriptions,
        FakeTrainingDayRepository trainingDays,
        FakeTaskRepository tasks) =>
        new(transactions, obligations, creditCards, subscriptions, trainingDays, tasks, CreateMapper());

    [Fact]
    public async Task GetTodayAsync_calcula_el_saldo_a_partir_de_las_transacciones()
    {
        var transactions = new FakeTransactionRepository();
        await transactions.AddAsync(new Transaction { Amount = 1000, Type = "income", Category = "Sueldo", Date = DateTime.UtcNow });
        await transactions.AddAsync(new Transaction { Amount = 300, Type = "expense", Category = "Comida", Date = DateTime.UtcNow });

        var service = CreateService(
            transactions, new FakeObligationRepository(), new FakeCreditCardRepository(),
            new FakeSubscriptionRepository(), new FakeTrainingDayRepository(), new FakeTaskRepository());

        var result = await service.GetTodayAsync();

        Assert.Equal(700, result.Balance);
        Assert.Equal(7, result.BalanceTrend.Count);
    }

    [Fact]
    public async Task GetTodayAsync_incluye_obligaciones_no_pagadas_dentro_de_la_ventana()
    {
        var obligations = new FakeObligationRepository();
        await obligations.AddAsync(new Obligation
        {
            Name = "Alquiler",
            Amount = 500,
            DueDate = DateTime.UtcNow.AddDays(3),
            IsPaid = false,
        });
        await obligations.AddAsync(new Obligation
        {
            Name = "Ya pagada",
            Amount = 100,
            DueDate = DateTime.UtcNow.AddDays(3),
            IsPaid = true,
        });

        var service = CreateService(
            new FakeTransactionRepository(), obligations, new FakeCreditCardRepository(),
            new FakeSubscriptionRepository(), new FakeTrainingDayRepository(), new FakeTaskRepository());

        var result = await service.GetTodayAsync();

        var due = Assert.Single(result.UpcomingDueDates);
        Assert.Equal("Alquiler", due.Name);
        Assert.Equal(UpcomingDueType.Obligation, due.Type);
    }

    [Fact]
    public async Task GetTodayAsync_devuelve_el_entrenamiento_del_dia_actual()
    {
        var trainingDays = new FakeTrainingDayRepository();
        var today = DateTime.UtcNow.DayOfWeek;
        await trainingDays.AddAsync(new TrainingDay { DayOfWeek = today, Title = "Entrenamiento de hoy" });
        await trainingDays.AddAsync(new TrainingDay { DayOfWeek = today == DayOfWeek.Sunday ? DayOfWeek.Monday : DayOfWeek.Sunday, Title = "Otro día" });

        var service = CreateService(
            new FakeTransactionRepository(), new FakeObligationRepository(), new FakeCreditCardRepository(),
            new FakeSubscriptionRepository(), trainingDays, new FakeTaskRepository());

        var result = await service.GetTodayAsync();

        Assert.NotNull(result.TodayTraining);
        Assert.Equal("Entrenamiento de hoy", result.TodayTraining!.Title);
    }

    [Fact]
    public async Task GetTodayAsync_incluye_tareas_urgentes_no_completadas()
    {
        var tasks = new FakeTaskRepository();
        await tasks.AddAsync(new TaskItem { Title = "Urgente", Status = TaskItemStatus.Pending, Date = DateTime.UtcNow });
        await tasks.AddAsync(new TaskItem { Title = "Completada", Status = TaskItemStatus.Done, Date = DateTime.UtcNow });
        await tasks.AddAsync(new TaskItem { Title = "Lejana", Status = TaskItemStatus.Pending, Date = DateTime.UtcNow.AddDays(10) });

        var service = CreateService(
            new FakeTransactionRepository(), new FakeObligationRepository(), new FakeCreditCardRepository(),
            new FakeSubscriptionRepository(), new FakeTrainingDayRepository(), tasks);

        var result = await service.GetTodayAsync();

        var urgentTask = Assert.Single(result.UrgentTasks);
        Assert.Equal("Urgente", urgentTask.Title);
    }

    [Fact]
    public async Task GetTodayAsync_calcula_el_monto_a_pagar_de_la_tarjeta_con_el_ciclo_ya_cerrado()
    {
        var today = DateTime.UtcNow.Date;
        // Mismo criterio que en CreditCardServiceTests: offsets de 10 días
        // (no 1) para que el resultado no dependa de qué día del mes corre el test.
        var statementDay = today.AddDays(-10).Day;
        var paymentDay = today.AddDays(10).Day;
        var lastStatementDate = today.AddDays(-10);

        var creditCards = new FakeCreditCardRepository();
        await creditCards.AddAsync(new CreditCard
        {
            Name = "Visa",
            StatementDay = statementDay,
            PaymentDay = paymentDay,
            CreditLimit = 1000,
        });
        var card = (await creditCards.GetAllAsync())[0];

        var transactions = new FakeTransactionRepository();
        // Dentro del ciclo que ya cerró (entre el corte anterior y el último corte) -> cuenta.
        await transactions.AddAsync(new Transaction { Amount = 100, Type = "expense", Category = "Comida", Date = lastStatementDate.AddDays(-15), CreditCardId = card.Id });
        await transactions.AddAsync(new Transaction { Amount = 50, Type = "expense", Category = "Transporte", Date = lastStatementDate, CreditCardId = card.Id });
        // Ciclo todavía abierto (después del último corte) -> no cuenta para esta cuota.
        await transactions.AddAsync(new Transaction { Amount = 999, Type = "expense", Category = "Ciclo abierto", Date = lastStatementDate.AddDays(5), CreditCardId = card.Id });
        // Ciclo más viejo, ya pagado en una cuota anterior -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 777, Type = "expense", Category = "Ciclo viejo", Date = lastStatementDate.AddDays(-45), CreditCardId = card.Id });
        // Ingreso dentro del rango -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 300, Type = "income", Category = "Reembolso", Date = lastStatementDate.AddDays(-10), CreditCardId = card.Id });
        // Gasto de otra tarjeta dentro del rango -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 555, Type = "expense", Category = "Otra tarjeta", Date = lastStatementDate.AddDays(-10), CreditCardId = 999 });

        var service = CreateService(
            transactions, new FakeObligationRepository(), creditCards,
            new FakeSubscriptionRepository(), new FakeTrainingDayRepository(), new FakeTaskRepository());

        var result = await service.GetTodayAsync();

        var due = Assert.Single(result.UpcomingDueDates);
        Assert.Equal(UpcomingDueType.CreditCard, due.Type);
        Assert.Equal(150, due.Amount);
    }
}
