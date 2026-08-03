namespace KatameApi.DTOs.Training;

public class UpdateTrainingDayDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string Title { get; set; } = string.Empty;
}
