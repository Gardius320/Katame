namespace KatameApi.DTOs.Finance;

public class CreateSavingsGoalDto
{
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MonthlyContributionTarget { get; set; }
}
