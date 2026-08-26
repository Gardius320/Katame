namespace KatameApi.DTOs.Finance;

public class SavingsGoalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MonthlyContributionTarget { get; set; }
    public int CurrentStreakMonths { get; set; }
    public int LongestStreakMonths { get; set; }
}
