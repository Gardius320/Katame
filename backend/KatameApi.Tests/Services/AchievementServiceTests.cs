using KatameApi.Models;
using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class AchievementServiceTests
{
    private static AchievementService CreateService(
        FakeUserAchievementRepository? achievements = null,
        FakeSavingsGoalRepository? goals = null,
        FakeTrainingStreakRepository? trainingStreak = null,
        FakeTrainingCompletionRepository? trainingCompletions = null,
        FakeTransactionRepository? transactions = null) =>
        new(
            achievements ?? new FakeUserAchievementRepository(),
            goals ?? new FakeSavingsGoalRepository(),
            trainingStreak ?? new FakeTrainingStreakRepository(),
            trainingCompletions ?? new FakeTrainingCompletionRepository(),
            transactions ?? new FakeTransactionRepository());

    private static Transaction Expense(DateTime date, string category, decimal amount) =>
        new() { Type = TransactionType.Expense, Category = category, Amount = amount, Date = date };

    [Fact]
    public async Task GetAllAsync_devuelve_el_catalogo_completo_sin_nada_desbloqueado_por_defecto()
    {
        var service = CreateService();

        var achievements = await service.GetAllAsync();

        Assert.Equal(AchievementCatalog.All.Count, achievements.Count);
        Assert.All(achievements, a => Assert.False(a.Unlocked));
    }

    [Fact]
    public async Task EvaluateAndUnlockAsync_desbloquea_primera_meta_cumplida_al_llegar_al_100_por_ciento()
    {
        var goals = new FakeSavingsGoalRepository();
        await goals.AddAsync(new SavingsGoal { Name = "Viaje", TargetAmount = 1000, CurrentAmount = 1000 });

        var service = CreateService(goals: goals);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.Contains(newlyUnlocked, a => a.Key == "primera_meta_cumplida");
    }

    [Fact]
    public async Task EvaluateAndUnlockAsync_no_repite_un_logro_que_ya_estaba_desbloqueado()
    {
        var goals = new FakeSavingsGoalRepository();
        await goals.AddAsync(new SavingsGoal { Name = "Viaje", TargetAmount = 1000, CurrentAmount = 1000 });
        var achievements = new FakeUserAchievementRepository();

        var service = CreateService(achievements: achievements, goals: goals);

        var first = await service.EvaluateAndUnlockAsync();
        var second = await service.EvaluateAndUnlockAsync();

        Assert.Contains(first, a => a.Key == "primera_meta_cumplida");
        Assert.Empty(second);
    }

    [Theory]
    [InlineData(2, false, false)]
    [InlineData(3, true, false)]
    [InlineData(6, true, true)]
    public async Task EvaluateAndUnlockAsync_desbloquea_rachas_de_ahorro_segun_el_record_de_la_meta(
        int longestStreakMonths, bool expectsThreeMonths, bool expectsSixMonths)
    {
        var goals = new FakeSavingsGoalRepository();
        await goals.AddAsync(new SavingsGoal
        {
            Name = "Emergencia",
            TargetAmount = 5000,
            CurrentAmount = 100,
            LongestStreakMonths = longestStreakMonths,
        });

        var service = CreateService(goals: goals);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.Equal(expectsThreeMonths, newlyUnlocked.Any(a => a.Key == "racha_ahorro_3"));
        Assert.Equal(expectsSixMonths, newlyUnlocked.Any(a => a.Key == "racha_ahorro_6"));
    }

    [Fact]
    public async Task EvaluateAndUnlockAsync_desbloquea_primer_entrenamiento_con_una_sola_marca()
    {
        var completions = new FakeTrainingCompletionRepository();
        await completions.AddAsync(new TrainingCompletion { Date = DateTime.UtcNow.Date });

        var service = CreateService(trainingCompletions: completions);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.Contains(newlyUnlocked, a => a.Key == "primer_entrenamiento");
        Assert.DoesNotContain(newlyUnlocked, a => a.Key == "veinticinco_entrenamientos");
    }

    [Fact]
    public async Task EvaluateAndUnlockAsync_desbloquea_veinticinco_entrenamientos_al_llegar_al_conteo()
    {
        var completions = new FakeTrainingCompletionRepository();
        for (var i = 0; i < 25; i++)
        {
            await completions.AddAsync(new TrainingCompletion { Date = DateTime.UtcNow.Date.AddDays(-i) });
        }

        var service = CreateService(trainingCompletions: completions);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.Contains(newlyUnlocked, a => a.Key == "veinticinco_entrenamientos");
    }

    [Theory]
    [InlineData(6, false, false)]
    [InlineData(7, true, false)]
    [InlineData(30, true, true)]
    public async Task EvaluateAndUnlockAsync_desbloquea_rachas_de_entrenamiento_segun_el_record(
        int longestStreakDays, bool expectsSevenDays, bool expectsThirtyDays)
    {
        var streak = new FakeTrainingStreakRepository();
        await streak.UpdateLongestIfHigherAsync(longestStreakDays);

        var service = CreateService(trainingStreak: streak);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.Equal(expectsSevenDays, newlyUnlocked.Any(a => a.Key == "racha_entrenamiento_7"));
        Assert.Equal(expectsThirtyDays, newlyUnlocked.Any(a => a.Key == "racha_entrenamiento_30"));
    }

    [Fact]
    public async Task EvaluateAndUnlockAsync_no_desbloquea_mes_sin_gastos_hormiga_sin_suficiente_actividad_el_mes_pasado()
    {
        var today = DateTime.UtcNow.Date;
        var lastMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-15);

        var transactions = new FakeTransactionRepository();
        // Solo 2 gastos el mes pasado: no alcanza para evaluar el mes.
        await transactions.AddAsync(Expense(lastMonth, "Café", 5));
        await transactions.AddAsync(Expense(lastMonth, "Café", 5));

        var service = CreateService(transactions: transactions);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.DoesNotContain(newlyUnlocked, a => a.Key == "mes_sin_gastos_hormiga");
    }

    [Fact]
    public async Task EvaluateAndUnlockAsync_desbloquea_mes_sin_gastos_hormiga_si_el_mes_pasado_no_tuvo_categorias_hormiga()
    {
        var today = DateTime.UtcNow.Date;
        var lastMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-15);

        var transactions = new FakeTransactionRepository();
        // Suficiente actividad, pero todos los montos son parecidos entre sí
        // -- ninguna categoría queda marcada como hormiga.
        await transactions.AddAsync(Expense(lastMonth, "Mercado", 50));
        await transactions.AddAsync(Expense(lastMonth, "Mercado", 48));
        await transactions.AddAsync(Expense(lastMonth, "Transporte", 52));
        await transactions.AddAsync(Expense(lastMonth, "Transporte", 49));
        await transactions.AddAsync(Expense(lastMonth, "Servicios", 51));

        var service = CreateService(transactions: transactions);

        var newlyUnlocked = await service.EvaluateAndUnlockAsync();

        Assert.Contains(newlyUnlocked, a => a.Key == "mes_sin_gastos_hormiga");
    }
}
