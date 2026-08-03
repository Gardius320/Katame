namespace KatameApi.DTOs.Training;

public class ExerciseDto
{
    public int Id { get; set; }
    public int TrainingDayId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SetsReps { get; set; } = string.Empty;
}
