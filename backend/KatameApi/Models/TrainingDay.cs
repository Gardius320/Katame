namespace KatameApi.Models;

public class TrainingDay : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Exercise> Exercises { get; set; } = new();
}
