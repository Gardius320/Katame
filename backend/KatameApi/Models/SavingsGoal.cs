namespace KatameApi.Models;

public class SavingsGoal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? DueDate { get; set; }
}
