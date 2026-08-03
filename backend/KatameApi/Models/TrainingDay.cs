namespace KatameApi.Models;

public class TrainingDay
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Exercise> Exercises { get; set; } = new();
}
