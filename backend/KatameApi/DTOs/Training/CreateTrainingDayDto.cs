namespace KatameApi.DTOs.Training;

public class CreateTrainingDayDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string Title { get; set; } = string.Empty;
}
