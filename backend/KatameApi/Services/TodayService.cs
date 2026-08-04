using AutoMapper;
using KatameApi.DTOs.Tasks;
using KatameApi.DTOs.Today;
using KatameApi.DTOs.Training;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class TodayService : ITodayService
{
    private const int UpcomingWindowDays = 14;
    private const int UrgentTaskWindowDays = 1;
    private const int BalanceTrendDays = 7;

    private readonly ITransactionRepository _transactionRepository;
    private readonly IObligationRepository _obligationRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITrainingDayRepository _trainingDayRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TodayService(
        ITransactionRepository transactionRepository,
        IObligationRepository obligationRepository,
        ICreditCardRepository creditCardRepository,
        ISubscriptionRepository subscriptionRepository,
        ITrainingDayRepository trainingDayRepository,
        ITaskRepository taskRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _obligationRepository = obligationRepository;
        _creditCardRepository = creditCardRepository;
        _subscriptionRepository = subscriptionRepository;
        _trainingDayRepository = trainingDayRepository;
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<TodayDto> GetTodayAsync()
    {
        var today = DateTime.UtcNow.Date;

        var transactions = await _transactionRepository.GetAllAsync(new TransactionFilter());
        var balance = transactions.Sum(t => t.Type == "income" ? t.Amount : -t.Amount);
        var balanceTrend = BuildBalanceTrend(transactions, today);

        var upcomingDueDates = await BuildUpcomingDueDatesAsync(today);

        var trainingDays = await _trainingDayRepository.GetAllAsync();
        var todayTraining = trainingDays.FirstOrDefault(d => d.DayOfWeek == today.DayOfWeek);

        var tasks = await _taskRepository.GetAllAsync();
        var urgentTasks = tasks
            .Where(t => t.Status != TaskItemStatus.Done
                && t.Date.HasValue
                && t.Date.Value.Date <= today.AddDays(UrgentTaskWindowDays))
            .OrderBy(t => t.Date)
            .ToList();

        return new TodayDto
        {
            Balance = balance,
            BalanceTrend = balanceTrend,
            UpcomingDueDates = upcomingDueDates,
            TodayTraining = todayTraining is null ? null : _mapper.Map<TrainingDayDto>(todayTraining),
            UrgentTasks = _mapper.Map<List<TaskItemDto>>(urgentTasks),
        };
    }

    private static List<BalanceTrendPointDto> BuildBalanceTrend(List<Transaction> transactions, DateTime today)
    {
        var startDate = today.AddDays(-(BalanceTrendDays - 1));
        var byDay = transactions
            .Where(t => t.Date.Date >= startDate && t.Date.Date <= today)
            .GroupBy(t => t.Date.Date)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Type == "income" ? t.Amount : -t.Amount));

        var trend = new List<BalanceTrendPointDto>();
        for (var day = startDate; day <= today; day = day.AddDays(1))
        {
            trend.Add(new BalanceTrendPointDto
            {
                Date = day,
                Amount = byDay.TryGetValue(day, out var amount) ? amount : 0,
            });
        }

        return trend;
    }

    private async Task<List<UpcomingDueDto>> BuildUpcomingDueDatesAsync(DateTime today)
    {
        var windowEnd = today.AddDays(UpcomingWindowDays);
        var result = new List<UpcomingDueDto>();

        var obligations = await _obligationRepository.GetAllAsync();
        result.AddRange(obligations
            .Where(o => !o.IsPaid && o.DueDate.Date <= windowEnd)
            .Select(o => new UpcomingDueDto
            {
                Type = UpcomingDueType.Obligation,
                Name = o.Name,
                DueDate = o.DueDate,
                Amount = o.Amount,
            }));

        var creditCards = await _creditCardRepository.GetAllAsync();
        foreach (var card in creditCards)
        {
            var nextPaymentDate = GetNextOccurrence(today, card.PaymentDay);
            if (nextPaymentDate <= windowEnd)
            {
                result.Add(new UpcomingDueDto
                {
                    Type = UpcomingDueType.CreditCard,
                    Name = card.Name,
                    DueDate = nextPaymentDate,
                    Amount = null,
                });
            }
        }

        var subscriptions = await _subscriptionRepository.GetAllAsync();
        result.AddRange(subscriptions
            .Where(s => s.RenewalDate.Date >= today && s.RenewalDate.Date <= windowEnd)
            .Select(s => new UpcomingDueDto
            {
                Type = UpcomingDueType.Subscription,
                Name = s.Name,
                DueDate = s.RenewalDate,
                Amount = s.Amount,
            }));

        return result.OrderBy(d => d.DueDate).ToList();
    }

    private static DateTime GetNextOccurrence(DateTime today, int dayOfMonth)
    {
        var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
        var clampedDay = Math.Min(dayOfMonth, daysInCurrentMonth);
        var candidate = new DateTime(today.Year, today.Month, clampedDay, 0, 0, 0, DateTimeKind.Utc);

        if (candidate.Date >= today)
        {
            return candidate;
        }

        var nextMonth = today.AddMonths(1);
        var daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
        var clampedNextDay = Math.Min(dayOfMonth, daysInNextMonth);
        return new DateTime(nextMonth.Year, nextMonth.Month, clampedNextDay, 0, 0, 0, DateTimeKind.Utc);
    }
}
