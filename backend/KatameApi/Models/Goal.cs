namespace KatameApi.Models;

public class Goal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime? DueDate { get; set; }
}
