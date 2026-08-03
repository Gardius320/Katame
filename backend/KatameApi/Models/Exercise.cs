namespace KatameApi.Models;

public class Exercise
{
    public int Id { get; set; }
    public int TrainingDayId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SetsReps { get; set; } = string.Empty;
}
