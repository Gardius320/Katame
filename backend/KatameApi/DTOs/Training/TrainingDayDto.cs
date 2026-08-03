namespace KatameApi.DTOs.Training;

public class TrainingDayDto
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<ExerciseDto> Exercises { get; set; } = new();
}
