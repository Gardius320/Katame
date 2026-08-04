namespace KatameApi.DTOs.Goals;

public class CreateGoalDto
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime? DueDate { get; set; }
}
