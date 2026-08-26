namespace KatameApi.DTOs.Training;

public class TrainingStreakDto
{
    public int CurrentStreakDays { get; set; }
    public int LongestStreakDays { get; set; }

    // Solo lo usa el frontend para decidir si vale la pena mostrar el aviso
    // animado (true la primera vez que se marca el día; false si se vuelve a
    // llamar al endpoint el mismo día, ej. por un reintento de red).
    public bool IsNewCompletion { get; set; }
}
