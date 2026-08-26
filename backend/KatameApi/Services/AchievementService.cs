using KatameApi.DTOs.Achievements;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class AchievementService : IAchievementService
{
    // Con menos gastos que esto en el mes anterior no hay suficiente
    // información para afirmar "no tuviste gastos hormiga" -- ver
    // IsAntExpenseFreeLastMonthAsync.
    private const int MinExpensesToEvaluateMonth = 5;

    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly ISavingsGoalRepository _savingsGoalRepository;
    private readonly ITrainingStreakRepository _trainingStreakRepository;
    private readonly ITrainingCompletionRepository _trainingCompletionRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AchievementService(
        IUserAchievementRepository userAchievementRepository,
        ISavingsGoalRepository savingsGoalRepository,
        ITrainingStreakRepository trainingStreakRepository,
        ITrainingCompletionRepository trainingCompletionRepository,
        ITransactionRepository transactionRepository)
    {
        _userAchievementRepository = userAchievementRepository;
        _savingsGoalRepository = savingsGoalRepository;
        _trainingStreakRepository = trainingStreakRepository;
        _trainingCompletionRepository = trainingCompletionRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<List<AchievementDto>> GetAllAsync()
    {
        var unlocked = (await _userAchievementRepository.GetAllAsync())
            .ToDictionary(a => a.Key, a => a.UnlockedAt);

        return AchievementCatalog.All.Select(definition => ToDto(definition, unlocked)).ToList();
    }

    public async Task<List<AchievementDto>> EvaluateAndUnlockAsync()
    {
        var alreadyUnlockedKeys = (await _userAchievementRepository.GetAllAsync())
            .Select(a => a.Key)
            .ToHashSet();

        var metKeys = await ComputeMetKeysAsync();
        var newlyUnlocked = new List<AchievementDto>();

        foreach (var definition in AchievementCatalog.All)
        {
            if (alreadyUnlockedKeys.Contains(definition.Key) || !metKeys.Contains(definition.Key))
            {
                continue;
            }

            if (await _userAchievementRepository.UnlockAsync(definition.Key))
            {
                newlyUnlocked.Add(new AchievementDto
                {
                    Key = definition.Key,
                    Category = definition.Category,
                    Title = definition.Title,
                    Description = definition.Description,
                    Unlocked = true,
                    UnlockedAt = DateTime.UtcNow,
                });
            }
        }

        return newlyUnlocked;
    }

    private async Task<HashSet<string>> ComputeMetKeysAsync()
    {
        var met = new HashSet<string>();

        var goals = await _savingsGoalRepository.GetAllAsync();
        if (goals.Any(g => g.TargetAmount > 0 && g.CurrentAmount >= g.TargetAmount))
        {
            met.Add("primera_meta_cumplida");
        }

        if (goals.Any(g => g.LongestStreakMonths >= 3))
        {
            met.Add("racha_ahorro_3");
        }

        if (goals.Any(g => g.LongestStreakMonths >= 6))
        {
            met.Add("racha_ahorro_6");
        }

        if (await IsAntExpenseFreeLastMonthAsync())
        {
            met.Add("mes_sin_gastos_hormiga");
        }

        var completionCount = (await _trainingCompletionRepository.GetAllDatesAsync()).Count;
        if (completionCount >= 1)
        {
            met.Add("primer_entrenamiento");
        }

        if (completionCount >= 25)
        {
            met.Add("veinticinco_entrenamientos");
        }

        var longestTrainingStreak = await _trainingStreakRepository.GetLongestAsync();
        if (longestTrainingStreak >= 7)
        {
            met.Add("racha_entrenamiento_7");
        }

        if (longestTrainingStreak >= 30)
        {
            met.Add("racha_entrenamiento_30");
        }

        return met;
    }

    // Evalúa el mes calendario ANTERIOR al actual (no el que está en curso,
    // porque todavía no terminó y no se puede afirmar que vaya a cerrar sin
    // gastos hormiga). Si no hubo suficiente actividad ese mes, no cuenta ni
    // a favor ni en contra -- no se desbloquea todavía.
    private async Task<bool> IsAntExpenseFreeLastMonthAsync()
    {
        var today = DateTime.UtcNow.Date;
        var firstOfThisMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var firstOfLastMonth = firstOfThisMonth.AddMonths(-1);
        var lastOfLastMonth = firstOfThisMonth.AddDays(-1);

        var expenses = (await _transactionRepository.GetAllAsync(new TransactionFilter
        {
            StartDate = firstOfLastMonth,
            EndDate = lastOfLastMonth,
        })).Where(t => t.Type == TransactionType.Expense).ToList();

        if (expenses.Count < MinExpensesToEvaluateMonth)
        {
            return false;
        }

        return AntExpenseAnalyzer.Analyze(expenses).Count == 0;
    }

    private static AchievementDto ToDto(AchievementDefinition definition, Dictionary<string, DateTime> unlocked)
    {
        var isUnlocked = unlocked.TryGetValue(definition.Key, out var unlockedAt);

        return new AchievementDto
        {
            Key = definition.Key,
            Category = definition.Category,
            Title = definition.Title,
            Description = definition.Description,
            Unlocked = isUnlocked,
            UnlockedAt = isUnlocked ? unlockedAt : null,
        };
    }
}
