using KatameApi.DTOs.Tasks;
using KatameApi.DTOs.Training;

namespace KatameApi.DTOs.Today;

public class TodayDto
{
    public decimal Balance { get; set; }
    public List<BalanceTrendPointDto> BalanceTrend { get; set; } = new();
    public List<UpcomingDueDto> UpcomingDueDates { get; set; } = new();
    public TrainingDayDto? TodayTraining { get; set; }
    public List<TaskItemDto> UrgentTasks { get; set; } = new();
}
